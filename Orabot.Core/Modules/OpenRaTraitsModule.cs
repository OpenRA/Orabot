using System.Threading.Tasks;
using Discord.Commands;
using Orabot.Core.Transformers.DocumentationToEmbedTransformers;

namespace Orabot.Core.Modules
{
	public class OpenRaTraitsModule : ModuleBase<SocketCommandContext>
	{
		private readonly TraitToEmbedTransformer _traitToEmbedTransformer;

		public OpenRaTraitsModule(TraitToEmbedTransformer traitToEmbedTransformer)
		{
			_traitToEmbedTransformer = traitToEmbedTransformer;
		}

		[Command("traits")]
		[Summary("Provides a link to the OpenRA Traits documentation page. Can be used with an optional trait name to link directly.")]
		public async Task Traits(string traitName = null)
		{
			var embed = await _traitToEmbedTransformer.CreateEmbed(traitName, "release");
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("traits-pt")]
		[Summary("Provides a link to the OpenRA playtest Traits documentation page. Can be used with an optional trait name to link directly.")]
		public async Task TraitsPt(string traitName = null)
		{
			var embed = await _traitToEmbedTransformer.CreateEmbed(traitName, "playtest");
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("traits-dev")]
		[Summary("Provides a link to the OpenRA development Traits documentation page. Can be used with an optional trait name to link directly.")]
		public async Task TraitsDev(string traitName = null)
		{
			var embed = await _traitToEmbedTransformer.CreateEmbed(traitName, "development");
			if (embed != null)
				await ReplyAsync("", false, embed);
		}
	}
}
