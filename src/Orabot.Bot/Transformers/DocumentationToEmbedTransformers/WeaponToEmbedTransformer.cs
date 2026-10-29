using Discord;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public class WeaponToEmbedTransformer : BaseDocumentationEmbedTransformer
	{
		public WeaponToEmbedTransformer(IConfiguration configuration)
			: base(configuration, "Weapons") { }

		internal async Task<Embed> CreateEmbed(string weaponName, string version)
		{
			var (isPresent, description) = await TryGetInfo(version, weaponName);
			var targetUrl = GetTargetUrl(version, isPresent ? weaponName : null);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Weapons page" + (isPresent ? $", weapon {weaponName}" : string.Empty),
					Url = targetUrl,
					IconUrl = OpenRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = description ?? "This documentation is aimed at modders. It displays a template for weapon definitions as well as its contained types (warheads and projectiles) with default values and developer commentary."
			};

			return embedBuilder.Build();
		}
	}
}
