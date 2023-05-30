using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Orabot.Core.Modules
{
	public class OpenRaWeaponsModule : ModuleBase<SocketCommandContext>
	{
		private readonly IRestClient _restClient;
		private readonly string _openRaIconUrl;
		private readonly string _weaponsReleasePageUrl;
		private readonly string _weaponsPlaytestPageUrl;
		private readonly string _weaponsDevelopmentPageUrl;

		public OpenRaWeaponsModule(IRestClient restClient, IConfiguration configuration)
		{
			_restClient = restClient;

			_openRaIconUrl = configuration["OpenRaFaviconUrl"];

			var traitsPages = configuration.GetRequiredSection("Weapons");
			_weaponsReleasePageUrl = traitsPages["ReleasePageUrl"];
			_weaponsPlaytestPageUrl = traitsPages["PlaytestPageUrl"];
			_weaponsDevelopmentPageUrl = traitsPages["DevelopmentPageUrl"];
		}

		[Command("weapons")]
		[Summary("Provides a link to the OpenRA Weapons documentation page. Can be used with an optional weapon name to link directly.")]
		public async Task Weapons(string weaponName = null)
		{
			var embed = await BuildWeaponsPageEmbed(_weaponsReleasePageUrl, weaponName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("weapons-pt")]
		[Summary("Provides a link to the OpenRA playtest Weapons documentation page. Can be used with an optional weapon name to link directly.")]
		public async Task WeaponsPt(string weaponName = null)
		{
			var embed = await BuildWeaponsPageEmbed(_weaponsPlaytestPageUrl, weaponName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("weapons-dev")]
		[Summary("Provides a link to the OpenRA development Weapons documentation page. Can be used with an optional weapon name to link directly.")]
		public async Task WeaponsDev(string weaponName = null)
		{
			var embed = await BuildWeaponsPageEmbed(_weaponsDevelopmentPageUrl, weaponName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		#region Private methods

		private async Task<bool> CheckWeaponExists(string pageUrl, string weaponName)
		{
			var request = new RestRequest(pageUrl, Method.Get);
			var response = await _restClient.ExecuteAsync(request);
			return response.Content.Contains($"<a href=\"#{weaponName.ToLower()}\"");
		}

		private async Task<Embed> BuildWeaponsPageEmbed(string pageUrl, string weaponName)
		{
			if (string.IsNullOrWhiteSpace(pageUrl))
				return null;

			var hasName = !string.IsNullOrWhiteSpace(weaponName);
			if (hasName)
				hasName = await CheckWeaponExists(pageUrl, weaponName);

			var targetUrl = pageUrl + (hasName ? $"#{weaponName.ToLower()}" : string.Empty);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Weapons page" + (hasName ? $", weapon {weaponName}" : string.Empty),
					Url = targetUrl,
					IconUrl = _openRaIconUrl
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
