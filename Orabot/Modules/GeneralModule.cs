using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Orabot.Modules
{
	public class GeneralModule : ModuleBase<SocketCommandContext>
	{
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
			await ReplyAsync(message);
		}
	}
}
