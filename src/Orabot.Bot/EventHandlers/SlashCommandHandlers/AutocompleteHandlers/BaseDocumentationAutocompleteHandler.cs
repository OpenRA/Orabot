using Discord;
using Discord.WebSocket;
using Orabot.Core.LongRunningServices;
using Orabot.Core.Objects.DocsWebsite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Core.EventHandlers.SlashCommandHandlers.AutocompleteHandlers
{
	public abstract class BaseDocumentationAutocompleteHandler
	{
		private readonly DocsCachingService _docsCachingService;

		public BaseDocumentationAutocompleteHandler(DocsCachingService docsCachingService)
		{
			_docsCachingService = docsCachingService;
		}

		protected ParsedDocs GetDocsCache(string version)
		{
			return version switch
			{
				"playtest" => _docsCachingService.PlaytestDocs,
				"development" => _docsCachingService.DevelopmentDocs,
				_ => _docsCachingService.ReleaseDocs
			};
		}

		protected abstract IReadOnlyDictionary<string, DocsEntry> GetDocsEntries(ParsedDocs docsCache);

		protected async Task InvokeInner(SocketAutocompleteInteraction interaction)
		{
			var version = (string)interaction.Data.Options.FirstOrDefault(x => x.Name == "version")?.Value ?? "release";
			var docsCache = GetDocsCache(version);
			var entries = GetDocsEntries(docsCache);

			var currentValue = (string)interaction.Data.Current.Value;
			var filteredEntries = entries
				.Where(x => x.Key.Contains(currentValue, System.StringComparison.InvariantCultureIgnoreCase));

			// The Discord API limits us to 25 suggestions at a time.
			await interaction.RespondAsync(filteredEntries.Take(25).Select(x => new AutocompleteResult(x.Key, x.Value.Title)));
		}
	}
}
