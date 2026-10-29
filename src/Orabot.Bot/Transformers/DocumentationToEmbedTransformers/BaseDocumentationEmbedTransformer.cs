using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public abstract class BaseDocumentationEmbedTransformer
	{
		protected readonly string OpenRaIconUrl;
		protected readonly string ReleasePageUrl;
		protected readonly string PlaytestPageUrl;
		protected readonly string DevelopmentPageUrl;

		protected BaseDocumentationEmbedTransformer(IConfiguration configuration, string configurationSection)
		{
			var pages = configuration.GetRequiredSection(configurationSection);
			ReleasePageUrl = pages["ReleasePageUrl"];
			PlaytestPageUrl = pages["PlaytestPageUrl"];
			DevelopmentPageUrl = pages["DevelopmentPageUrl"];

			OpenRaIconUrl = configuration["OpenRaFaviconUrl"];
		}

		protected virtual string GetPageUrl(string version)
		{
			return version switch
			{
				"playtest" => PlaytestPageUrl,
				"development" => DevelopmentPageUrl,
				_ => ReleasePageUrl,
			};
		}

		protected virtual string GetTargetUrl(string version, string name)
		{
			return GetPageUrl(version) + (name != null ? $"#{name.ToLower()}" : string.Empty);
		}

		protected virtual async Task<(bool IsPresent, string Description)> TryGetInfo(string version, string name)
		{
			if (string.IsNullOrWhiteSpace(version) || string.IsNullOrWhiteSpace(name))
				return (false, null);

			var pageUrl = GetPageUrl(version);
			var web = new HtmlWeb();
			var doc = await web.LoadFromWebAsync(pageUrl);
			var node = doc.DocumentNode.Descendants("h3").FirstOrDefault(x => x.Attributes["id"].Value == name.ToLower());
			return node == null ? (false, null) : (true, node.NextSibling.NextSibling.InnerText);
		}
	}
}
