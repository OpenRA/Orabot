using Orabot.Core.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Orabot.Core.Integrations.DocsWebsite;
using System;
using System.Collections.Generic;
using Orabot.Core.Objects.DocsWebsite;
using System.Linq;

namespace Orabot.Core.LongRunningServices
{
	internal class DocsCachingService : ILongRunningService
	{
		private readonly int ScanInterval;
		private readonly IDocsApi _docsApi;

		public ParsedDocs ReleaseDocs { get; private set; }

		public ParsedDocs PlaytestDocs { get; private set; }

		public ParsedDocs DevelopmentDocs { get; private set; }

		public DocsCachingService(IConfiguration configuration, IDocsApi docsApi)
		{
			var conf = configuration.GetRequiredSection("DocsSearchIndex");
			ScanInterval = int.Parse(conf["ScanInterval"]);
			_docsApi = docsApi;
		}

		public async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					Console.WriteLine($"{DateTime.Now} Updating documentation cache...");

					ReleaseDocs = await ParseSearchIndex("release");
					await Task.Delay(2000);
					PlaytestDocs = await ParseSearchIndex("playtest");
					await Task.Delay(2000);
					DevelopmentDocs = await ParseSearchIndex("bleed");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}

				await Task.Delay(ScanInterval);
			}
		}

		async Task<ParsedDocs> ParseSearchIndex(string version)
		{
			var response = await _docsApi.GetSearchIndex(version);
			return new ParsedDocs
			{
				Traits = PrepareEntries(response.Docs, "traits/#"),
				Weapons = PrepareEntries(response.Docs, "weapons/#"),
				SpriteSequences = PrepareEntries(response.Docs, "sprite-sequences/#"),
				Lua = PrepareEntries(response.Docs, "lua/#")
			};
		}

		public static IReadOnlyDictionary<string, DocsEntry> PrepareEntries(DocsEntry[] docs, string filter)
		{
			return docs
				.Where(x => x.Location.StartsWith(filter))
				.DistinctBy(x => x.Title)
				.ToDictionary(x => x.Title, y => y);
		}
	}
}
