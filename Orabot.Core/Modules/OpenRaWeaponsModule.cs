using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Orabot.Core.Modules
{
	public class OpenRaWeaponsModule : ModuleBase<SocketCommandContext>
	{
		private const string WeaponsPageUrl = "https://docs.openra.net/en/latest/release/weapons/";
		private const string WeaponsPlaytestPageUrl = "https://docs.openra.net/en/latest/playtest/weapons/";

		private readonly IRestClient _restClient;
		private readonly string _openRaIconUrl;

		public OpenRaWeaponsModule(IRestClient restClient, IConfiguration configuration)
		{
			_restClient = restClient;
			_restClient.BaseUrl = new Uri(WeaponsPageUrl);
			_openRaIconUrl = configuration["OpenRaFaviconUrl"];
		}

		[Command("weapons")]
		[Summary("Provides a link to the OpenRA Weapons documentation page. Can be used with an optional weapon name to link directly.")]
		public async Task Weapons(string weaponName = null)
		{
			var embed = BuildWeaponsPageEmbed(WeaponsPageUrl, weaponName);
			await ReplyAsync("", false, embed);
		}

		[Command("weapons-pt")]
		[Summary("Provides a link to the OpenRA playtest Weapons documentation page. Can be used with an optional weapon name to link directly.")]
		public async Task WeaponsPt(string weaponName = null)
		{
			var embed = BuildWeaponsPageEmbed(WeaponsPlaytestPageUrl, weaponName);
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
