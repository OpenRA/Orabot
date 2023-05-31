using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Orabot.Core.Modules
{
	public class OpenRaTraitsModule : ModuleBase<SocketCommandContext>
	{
		private readonly IRestClient _restClient;
		private readonly string _openRaIconUrl;
		private readonly string _traitsReleasePageUrl;
		private readonly string _traitsPlaytestPageUrl;
		private readonly string _traitsDevelopmentPageUrl;

		public OpenRaTraitsModule(IRestClient restClient, IConfiguration configuration)
		{
			_restClient = restClient;

			_openRaIconUrl = configuration["OpenRaFaviconUrl"];

			var traitsPages = configuration.GetRequiredSection("Traits");
			_traitsReleasePageUrl = traitsPages["ReleasePageUrl"];
			_traitsPlaytestPageUrl = traitsPages["PlaytestPageUrl"];
			_traitsDevelopmentPageUrl = traitsPages["DevelopmentPageUrl"];
		}

		[Command("traits")]
		[Summary("Provides a link to the OpenRA Traits documentation page. Can be used with an optional trait name to link directly.")]
		public async Task Traits(string traitName = null)
		{
			var embed = await BuildTraitsPageEmbed(_traitsReleasePageUrl, traitName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("traits-pt")]
		[Summary("Provides a link to the OpenRA playtest Traits documentation page. Can be used with an optional trait name to link directly.")]
		public async Task TraitsPt(string traitName = null)
		{
			var embed = await BuildTraitsPageEmbed(_traitsPlaytestPageUrl, traitName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("traits-dev")]
		[Summary("Provides a link to the OpenRA development Traits documentation page. Can be used with an optional trait name to link directly.")]
		public async Task TraitsDev(string traitName = null)
		{
			var embed = await BuildTraitsPageEmbed(_traitsDevelopmentPageUrl, traitName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		#region Private methods

		private async Task<bool> CheckTraitExists(string pageUrl, string traitName)
		{
			var request = new RestRequest(pageUrl);
			var response = await _restClient.GetAsync(request);
			return response.Content?.Contains($"<a href=\"#{traitName.ToLower()}\"") ?? false;
		}

		private async Task<Embed> BuildTraitsPageEmbed(string pageUrl, string traitName)
		{
			if (string.IsNullOrWhiteSpace(pageUrl))
				return null;

			var hasName = !string.IsNullOrWhiteSpace(traitName);
			if (hasName)
				hasName = await CheckTraitExists(pageUrl, traitName);

			var targetUrl = pageUrl + (hasName ? $"#{traitName.ToLower()}" : string.Empty);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Traits page" + (hasName ? $", trait {traitName}" : string.Empty),
					Url = targetUrl,
					IconUrl = _openRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = "This documentation is aimed at modders. It displays all traits with default values and developer commentary."
			};

			return embedBuilder.Build();
		}

		#endregion
	}
}
