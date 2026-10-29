using Discord.WebSocket;
using Orabot.Bot.Features.Documentation.Rendering;
using Orabot.Core.Abstractions.EventHandlers;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Bot.Features.Documentation.Commands
{
	internal class TraitDocumentationCommandHandler : ISlashCommandHandler
	{
		private readonly TraitToEmbedTransformer _traitToEmbedTransformer;

		public TraitDocumentationCommandHandler(TraitToEmbedTransformer traitToEmbedTransformer)
		{
			_traitToEmbedTransformer = traitToEmbedTransformer;
		}

		public bool CanHandle(SocketSlashCommand command) => command.CommandName == "traits";

		public async Task InvokeAsync(SocketSlashCommand command)
		{
			var name = (string)command.Data.Options.FirstOrDefault(x => x.Name == "name")?.Value;
			var version = (string)command.Data.Options.FirstOrDefault(x => x.Name == "version")?.Value ?? "release";

			var embed = await _traitToEmbedTransformer.CreateEmbed(name, version);
			if (embed != null)
				await command.RespondAsync(embed: embed);
		}
	}
}
