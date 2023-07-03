using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Orabot.Core.Transformers.DocumentationToEmbedTransformers;

namespace Orabot.Core.Modules
{
	public class OpenRaTraitsModule : ModuleBase<SocketCommandContext>
	{
		private readonly string _traitsReleasePageUrl;
		private readonly string _traitsPlaytestPageUrl;
		private readonly string _traitsDevelopmentPageUrl;
		private readonly TraitToEmbedTransformer _traitToEmbedTransformer;

		public OpenRaTraitsModule(TraitToEmbedTransformer traitToEmbedTransformer, IConfiguration configuration)
		{
			var traitsPages = configuration.GetRequiredSection("Traits");
			_traitsReleasePageUrl = traitsPages["ReleasePageUrl"];
			_traitsPlaytestPageUrl = traitsPages["PlaytestPageUrl"];
			_traitsDevelopmentPageUrl = traitsPages["DevelopmentPageUrl"];
			_traitToEmbedTransformer = traitToEmbedTransformer;
		}

		[Command("traits")]
		[Summary("Provides a link to the OpenRA Traits documentation page. Can be used with an optional trait name to link directly.")]
		public async Task Traits(string traitName = null)
		{
			var embed = await _traitToEmbedTransformer.CreateEmbed(_traitsReleasePageUrl, traitName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("traits-pt")]
		[Summary("Provides a link to the OpenRA playtest Traits documentation page. Can be used with an optional trait name to link directly.")]
		public async Task TraitsPt(string traitName = null)
		{
			var embed = await _traitToEmbedTransformer.CreateEmbed(_traitsPlaytestPageUrl, traitName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("traits-dev")]
		[Summary("Provides a link to the OpenRA development Traits documentation page. Can be used with an optional trait name to link directly.")]
		public async Task TraitsDev(string traitName = null)
		{
			var embed = await _traitToEmbedTransformer.CreateEmbed(_traitsDevelopmentPageUrl, traitName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		#region Private methods


		#endregion
	}
}
