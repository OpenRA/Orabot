using Discord.WebSocket;
using Orabot.Core.Abstractions.EventHandlers;
using Orabot.Core.Transformers.DocumentationToEmbedTransformers;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Core.EventHandlers.SlashCommandHandlers
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
			var traitName = (string)command.Data.Options.FirstOrDefault(x => x.Name == "trait-name")?.Value;
			var version = (string)command.Data.Options.FirstOrDefault(x => x.Name == "version")?.Value ?? "release";

			var embed = await _traitToEmbedTransformer.CreateEmbed(traitName, version);
			if (embed != null)
				await command.RespondAsync(embed: embed);
		}
	}
}
