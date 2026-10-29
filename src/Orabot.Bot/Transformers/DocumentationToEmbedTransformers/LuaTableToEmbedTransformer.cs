using Discord;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Orabot.Core.Transformers.DocumentationToEmbedTransformers
{
	public class LuaTableToEmbedTransformer : BaseDocumentationEmbedTransformer
	{
		public LuaTableToEmbedTransformer(IConfiguration configuration)
			: base(configuration, "Lua") { }

		internal async Task<Embed> CreateEmbed(string tableName, string version)
		{
			var (isPresent, _) = await TryGetInfo(version, tableName);
			var targetUrl = GetTargetUrl(version, isPresent ? tableName : null);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Lua API page" + (isPresent ? $", table {tableName}" : string.Empty),
					Url = targetUrl,
					IconUrl = OpenRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = "This documentation is aimed at scripted map creators."
			};

			return embedBuilder.Build();
		}
	}
}
