using Discord;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public class SpriteSequenceToEmbedTransformer
	{
		private readonly string _spriteSequenceReleasePageUrl;
		private readonly string _spriteSequencePlaytestPageUrl;
		private readonly string _spriteSequenceDevelopmentPageUrl;
		private readonly string _openRaIconUrl;

		public SpriteSequenceToEmbedTransformer(IConfiguration configuration)
		{
			var luaPages = configuration.GetRequiredSection("SpriteSequences");
			_spriteSequenceReleasePageUrl = luaPages["ReleasePageUrl"];
			_spriteSequencePlaytestPageUrl = luaPages["PlaytestPageUrl"];
			_spriteSequenceDevelopmentPageUrl = luaPages["DevelopmentPageUrl"];

			_openRaIconUrl = configuration["OpenRaFaviconUrl"];
		}

		internal async Task<Embed> CreateEmbed(string tableName, string version)
		{
			string pageUrl = version switch
			{
				"playtest" => _spriteSequencePlaytestPageUrl,
				"development" => _spriteSequenceDevelopmentPageUrl,
				_ => _spriteSequenceReleasePageUrl,
			};

			var (isPresent, description) = await TryGetInfo(pageUrl, tableName);
			var targetUrl = pageUrl + (isPresent ? $"#{tableName.ToLower()}" : string.Empty);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Sprite Sequences page" + (isPresent ? $", {tableName}" : string.Empty),
					Url = targetUrl,
					IconUrl = _openRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = description
			};

			return embedBuilder.Build();
		}

		private async Task<(bool IsPresent, string Description)> TryGetInfo(string pageUrl, string tableName)
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
