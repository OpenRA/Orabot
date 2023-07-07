using Discord.WebSocket;
using Orabot.Core.Abstractions.EventHandlers;
using Orabot.Core.Transformers.DocumentationToEmbedTransformers;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Core.EventHandlers.SlashCommandHandlers
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
