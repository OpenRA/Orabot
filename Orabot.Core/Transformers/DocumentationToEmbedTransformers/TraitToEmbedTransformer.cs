using Discord;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public class TraitToEmbedTransformer
	{
		private readonly string _traitsReleasePageUrl;
		private readonly string _traitsPlaytestPageUrl;
		private readonly string _traitsDevelopmentPageUrl;
		private readonly string _openRaIconUrl;

		public TraitToEmbedTransformer(IConfiguration configuration)
		{
			var traitsPages = configuration.GetRequiredSection("Traits");
			_traitsReleasePageUrl = traitsPages["ReleasePageUrl"];
			_traitsPlaytestPageUrl = traitsPages["PlaytestPageUrl"];
			_traitsDevelopmentPageUrl = traitsPages["DevelopmentPageUrl"];

			_openRaIconUrl = configuration["OpenRaFaviconUrl"];
		}

		internal async Task<Embed> CreateEmbed(string traitName, string version)
		{
			string pageUrl = version switch
			{
				"playtest" => _traitsPlaytestPageUrl,
				"development" => _traitsDevelopmentPageUrl,
				_ => _traitsReleasePageUrl,
			};

			var (traitExists, traitDescription) = await TryGetTraitInfo(pageUrl, traitName);
			var targetUrl = pageUrl + (traitExists ? $"#{traitName.ToLower()}" : string.Empty);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Traits page" + (traitExists ? $", trait {traitName}" : string.Empty),
					Url = targetUrl,
					IconUrl = _openRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = traitDescription
			};

			return embedBuilder.Build();
		}

		private async Task<(bool TraitExists, string Description)> TryGetTraitInfo(string pageUrl, string traitName)
		{
			if (string.IsNullOrWhiteSpace(pageUrl) || string.IsNullOrWhiteSpace(traitName))
				return (false, null);

			var web = new HtmlWeb();
			var doc = await web.LoadFromWebAsync(pageUrl);
			var node = doc.DocumentNode.Descendants("h3").FirstOrDefault(x => x.Attributes["id"].Value == traitName.ToLower());
			return node == null ? (false, null) : (true, node.NextSibling.NextSibling.InnerText);
		}
	}
}
