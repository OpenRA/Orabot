using Discord;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public class TraitToEmbedTransformer : BaseDocumentationEmbedTransformer
	{
		public TraitToEmbedTransformer(IConfiguration configuration)
			: base(configuration, "Traits") { }

		internal async Task<Embed> CreateEmbed(string traitName, string version)
		{
			var (isPresent, description) = await TryGetInfo(version, traitName);
			var targetUrl = GetTargetUrl(version, isPresent ? traitName : null);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Traits page" + (isPresent ? $", trait {traitName}" : string.Empty),
					Url = targetUrl,
					IconUrl = OpenRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = description ?? "This documentation is aimed at modders. It displays all traits with default values and developer commentary."
			};

			return embedBuilder.Build();
		}
	}
}
