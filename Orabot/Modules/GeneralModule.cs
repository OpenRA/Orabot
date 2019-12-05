using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Orabot.Extensions;

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
			// This is set up to use a more streamlined look than previous versions and takes inspiration from the Markdown example at
			// https://gist.github.com/Almeeida/41a664d8d5f3a8855591c2f1e0e07b19
			await ReplyAsync($"```md\n{ string.Join("\n", _commands.Commands.Select(x => x.CustomToString())) }\n```");
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
