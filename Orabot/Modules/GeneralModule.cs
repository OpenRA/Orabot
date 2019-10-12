using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Orabot.Modules
{
	public class GeneralModule : ModuleBase<SocketCommandContext>
	{
		private readonly string[] _trustedRoles = ConfigurationManager.AppSettings["TrustedRoles"].Split(';').Select(x => x.Trim()).ToArray();
		private readonly CommandService _commands;

		public GeneralModule(CommandService commands)
		{
			_commands = commands;
		}

		[Command("help", true)]
		[Summary("Lists all available commands.")]
		public async Task Help()
		{
			await ReplyAsync($"```\n{string.Join("\n", _commands.Commands.Select(x => $"{x.Name} - {x.Summary}"))}\n```");
		}

		[Command("hi")]
		[Summary("Says hi.")]
		public async Task Hi()
		{
			await ReplyAsync($"Hi, {Context.User.Mention} !");
		}

		[Command("say")]
		[Summary("Repeats what you say.")]
		public async Task Say([Remainder]string message)
		{
			var userRoles = (Context.User as SocketGuildUser)?.Roles;
			if (userRoles != null && userRoles.Select(x => x.Name).Intersect(_trustedRoles).Any())
			{
				await ReplyAsync(message);
			}
			else
			{
				await ReplyAsync("No rights!");
			}
		}
	}
}
