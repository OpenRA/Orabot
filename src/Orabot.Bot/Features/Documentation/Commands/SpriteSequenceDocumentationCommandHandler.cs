using Discord.WebSocket;
using Orabot.Bot.Discord.Interactions;
using Orabot.Bot.Features.Documentation.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Bot.Features.Documentation.Commands
{
	internal class SpriteSequenceDocumentationCommandHandler : ISlashCommandHandler
	{
		readonly SpriteSequenceToEmbedTransformer _spriteSequenceToEmbedTransformer;

		public SpriteSequenceDocumentationCommandHandler(SpriteSequenceToEmbedTransformer spriteSequenceToEmbedTransformer)
		{
			_spriteSequenceToEmbedTransformer = spriteSequenceToEmbedTransformer;
		}

		public bool CanHandle(SocketSlashCommand command) => command.CommandName == "sprite-sequences";

		public async Task InvokeAsync(SocketSlashCommand command)
		{
			var name = (string)command.Data.Options.FirstOrDefault(x => x.Name == "name")?.Value;
			var version = (string)command.Data.Options.FirstOrDefault(x => x.Name == "version")?.Value ?? "release";

			var embed = await _spriteSequenceToEmbedTransformer.CreateEmbed(name, version);
			if (embed != null)
				await command.RespondAsync(embed: embed);
		}
	}
}
