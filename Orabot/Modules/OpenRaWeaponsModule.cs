using System;
using System.Configuration;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RestSharp;

namespace Orabot.Modules
{
	public class OpenRaWeaponsModule : ModuleBase<SocketCommandContext>
	{
		private const string WeaponsPageUrl = "https://github.com/OpenRA/OpenRA/wiki/Weapons";

		private static readonly string OpenRaIconUrl = ConfigurationManager.AppSettings["OpenRaFaviconUrl"];

		private readonly IRestClient _restClient;

		public OpenRaWeaponsModule(IRestClient restClient)
		{
			_restClient = restClient;
			_restClient.BaseUrl = new Uri(WeaponsPageUrl);
		}

		[Command("weapons")]
		[Summary("Provides a link to the GitHub Weapons Wiki page. Can be used with an optional weapon name to link directly.")]
		public async Task Weapons(string weaponName = null)
		{
			var embed = BuildWeaponsPageEmbed(WeaponsPageUrl, weaponName);
			await ReplyAsync("", false, embed);
		}

		#region Private methods

		private bool CheckWeaponExists(string weaponName)
		{
			var request = new RestRequest(Method.GET);
			var response = _restClient.Execute(request);
			return response.Content.Contains($"<a href=\"#{weaponName.ToLower()}\"");
		}

		private Embed BuildWeaponsPageEmbed(string pageUrl, string weaponName)
		{
			var hasName = !string.IsNullOrWhiteSpace(weaponName);
			if (hasName)
			{
				hasName = CheckWeaponExists(weaponName);
			}

			var targetUrl = pageUrl + (hasName ? $"#{weaponName}" : string.Empty);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Weapons page" + (hasName ? $", weapon {weaponName}" : string.Empty),
					Url = targetUrl,
					IconUrl = OpenRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = "This documentation is aimed at modders. It displays a template for weapon definitions as well as its contained types (warheads and projectiles) with default values and developer commentary."
			};

			return embedBuilder.Build();
		}

		#endregion
	}
}
