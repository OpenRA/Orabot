using Discord;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public class LuaTableToEmbedTransformer
	{
		private readonly string _luaReleasePageUrl;
		private readonly string _luaPlaytestPageUrl;
		private readonly string _luaDevelopmentPageUrl;
		private readonly string _openRaIconUrl;

		public LuaTableToEmbedTransformer(IConfiguration configuration)
		{
			var luaPages = configuration.GetRequiredSection("Lua");
			_luaReleasePageUrl = luaPages["ReleasePageUrl"];
			_luaPlaytestPageUrl = luaPages["PlaytestPageUrl"];
			_luaDevelopmentPageUrl = luaPages["DevelopmentPageUrl"];

			_openRaIconUrl = configuration["OpenRaFaviconUrl"];
		}

		internal async Task<Embed> CreateEmbed(string tableName, string version)
		{
			string pageUrl = version switch
			{
				"playtest" => _luaPlaytestPageUrl,
				"development" => _luaDevelopmentPageUrl,
				_ => _luaReleasePageUrl,
			};

			var (tableExists, tableDescription) = await TryGetTableInfo(pageUrl, tableName);
			var targetUrl = pageUrl + (tableExists ? $"#{tableName.ToLower()}" : string.Empty);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Lua API page" + (tableExists ? $", table {tableName}" : string.Empty),
					Url = targetUrl,
					IconUrl = _openRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = tableDescription
			};

			return embedBuilder.Build();
		}

		private async Task<(bool TableExists, string Description)> TryGetTableInfo(string pageUrl, string tableName)
		{
			if (string.IsNullOrWhiteSpace(pageUrl) || string.IsNullOrWhiteSpace(tableName))
				return (false, null);

			var web = new HtmlWeb();
			var doc = await web.LoadFromWebAsync(pageUrl);
			var node = doc.DocumentNode.Descendants("h3").FirstOrDefault(x => x.Attributes["id"].Value == tableName.ToLower());
			return node == null ? (false, null) : (true, node.NextSibling.NextSibling.InnerText);
		}
	}
}
