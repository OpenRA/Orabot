using System;
using System.Configuration;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RestSharp;

namespace Orabot.Modules
{
	public class OpenRaTraitsModule : ModuleBase<SocketCommandContext>
	{
		private const string TraitsPageUrl = "https://github.com/OpenRA/OpenRA/wiki/Traits";
		private const string TraitsPlaytestPageUrl = "https://github.com/OpenRA/OpenRA/wiki/Traits-(playtest)";

		private static readonly string OpenRaIconUrl = ConfigurationManager.AppSettings["OpenRaFaviconUrl"];

		private readonly IRestClient _restClient;

		public OpenRaTraitsModule(IRestClient restClient)
		{
			_restClient = restClient;
			_restClient.BaseUrl = new Uri(TraitsPageUrl);
		}

		[Command("traits")]
		[Summary("Provides a link to the GitHub Traits page. Can be used with an optional trait name to link directly.")]
		public async Task Traits(string traitName = null)
		{
			var embed = BuildTraitsPageEmbed(TraitsPageUrl, traitName);
			await ReplyAsync("", false, embed);
		}

		[Command("traits-pt")]
		[Summary("Provides a link to the GitHub playtest Traits page. Can be used with an optional trait name to link directly.")]
		public async Task TraitsPt(string traitName = null)
		{
			var embed = BuildTraitsPageEmbed(TraitsPlaytestPageUrl, traitName);
			await ReplyAsync("", false, embed);
		}

		#region Private methods

		private bool CheckTraitExists(string traitName)
		{
			var request = new RestRequest(Method.GET);
			var response = _restClient.Execute(request);
			return response.Content.Contains($"<a href=\"#{traitName.ToLower()}\"");
		}

		private Embed BuildTraitsPageEmbed(string pageUrl, string traitName)
		{
			var hasName = !string.IsNullOrWhiteSpace(traitName);
			if (hasName)
			{
				hasName = CheckTraitExists(traitName);
			}

			var targetUrl = pageUrl + (hasName ? $"#{traitName}" : string.Empty);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Traits page" + (hasName ? $", trait {traitName}" : string.Empty),
					Url = targetUrl,
					IconUrl = OpenRaIconUrl
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
