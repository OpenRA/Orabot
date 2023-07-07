using Discord;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public class SpriteSequenceToEmbedTransformer : BaseDocumentationEmbedTransformer
	{
		public SpriteSequenceToEmbedTransformer(IConfiguration configuration)
			: base(configuration, "SpriteSequences") { }

		internal async Task<Embed> CreateEmbed(string sequenceTypeName, string version)
		{
			var (isPresent, description) = await TryGetInfo(version, sequenceTypeName);
			var targetUrl = GetTargetUrl(version, isPresent ? sequenceTypeName : null);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Sprite Sequences page" + (isPresent ? $", {sequenceTypeName}" : string.Empty),
					Url = targetUrl,
					IconUrl = OpenRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = description ?? "This documentation is aimed at modders. It displays all sprite sequence types with their properties and their default values plus developer commentary."
			};

			return embedBuilder.Build();
		}
	}
}
