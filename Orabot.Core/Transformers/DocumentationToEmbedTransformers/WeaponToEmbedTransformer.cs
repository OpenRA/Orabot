using Discord;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public class WeaponToEmbedTransformer
	{
		private readonly string _weaponsReleasePageUrl;
		private readonly string _weaponsPlaytestPageUrl;
		private readonly string _weaponsDevelopmentPageUrl;
		private readonly string _openRaIconUrl;

		public WeaponToEmbedTransformer(IConfiguration configuration)
		{
			var weaponsPages = configuration.GetRequiredSection("Weapons");
			_weaponsReleasePageUrl = weaponsPages["ReleasePageUrl"];
			_weaponsPlaytestPageUrl = weaponsPages["PlaytestPageUrl"];
			_weaponsDevelopmentPageUrl = weaponsPages["DevelopmentPageUrl"];

			_openRaIconUrl = configuration["OpenRaFaviconUrl"];
		}

		internal async Task<Embed> CreateEmbed(string weaponName, string version)
		{
			string pageUrl = version switch
			{
				"playtest" => _weaponsPlaytestPageUrl,
				"development" => _weaponsDevelopmentPageUrl,
				_ => _weaponsReleasePageUrl,
			};

			var (weaponExists, weaponDescription) = await TryGetWeaponInfo(pageUrl, weaponName);
			var targetUrl = pageUrl + (weaponExists ? $"#{weaponName.ToLower()}" : string.Empty);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Weapons page" + (weaponExists ? $", weapon {weaponName}" : string.Empty),
					Url = targetUrl,
					IconUrl = _openRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = weaponDescription ?? "This documentation is aimed at modders. It displays a template for weapon definitions as well as its contained types (warheads and projectiles) with default values and developer commentary."
			};

			return embedBuilder.Build();
		}

		private async Task<(bool WeaponExists, string Description)> TryGetWeaponInfo(string pageUrl, string weaponName)
		{
			if (string.IsNullOrWhiteSpace(pageUrl) || string.IsNullOrWhiteSpace(weaponName))
				return (false, null);

			var web = new HtmlWeb();
			var doc = await web.LoadFromWebAsync(pageUrl);
			var node = doc.DocumentNode.Descendants("h3").FirstOrDefault(x => x.Attributes["id"].Value == weaponName.ToLower());
			return node == null ? (false, null) : (true, node.NextSibling.NextSibling.InnerText);
		}
	}
}
