using Discord;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public class TraitToEmbedTransformer
	{
		private readonly IRestClient _restClient;
		private readonly string _openRaIconUrl;

		public TraitToEmbedTransformer(IRestClient restClient, IConfiguration configuration)
		{
			_restClient = restClient;
			_openRaIconUrl = configuration["OpenRaFaviconUrl"];
		}

		internal async Task<Embed> CreateEmbed(string pageUrl, string traitName)
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

		private async Task<bool> CheckTraitExists(string pageUrl, string traitName)
		{
			var request = new RestRequest(pageUrl);
			var response = await _restClient.GetAsync(request);
			return response.Content?.Contains($"<a href=\"#{traitName.ToLower()}\"") ?? false;
		}
	}
}
