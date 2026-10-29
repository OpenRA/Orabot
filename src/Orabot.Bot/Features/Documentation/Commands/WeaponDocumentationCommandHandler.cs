using Discord.WebSocket;
using Orabot.Bot.Discord.Interactions;
using Orabot.Bot.Features.Documentation.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Bot.Features.Documentation.Commands
{
	internal class WeaponDocumentationCommandHandler : ISlashCommandHandler
	{
		private readonly WeaponToEmbedTransformer _weaponToEmbedTransformer;

		public WeaponDocumentationCommandHandler(WeaponToEmbedTransformer weaponToEmbedTransformer)
		{
			_weaponToEmbedTransformer = weaponToEmbedTransformer;
		}

		public bool CanHandle(SocketSlashCommand command) => command.CommandName == "weapons";

		public async Task InvokeAsync(SocketSlashCommand command)
		{
			var name = (string)command.Data.Options.FirstOrDefault(x => x.Name == "name")?.Value;
			var version = (string)command.Data.Options.FirstOrDefault(x => x.Name == "version")?.Value ?? "release";

			var embed = await _weaponToEmbedTransformer.CreateEmbed(name, version);
			if (embed != null)
				await command.RespondAsync(embed: embed);
		}
	}
}
