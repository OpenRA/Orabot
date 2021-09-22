using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Orabot.Core.Extensions;

namespace Orabot.Core.Modules
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
			foreach (var block in GenerateHelpBlocks())
			{
				await ReplyAsync(block);
			}
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

		#region Private methods

		private IEnumerable<string> GenerateHelpBlocks()
		{
			// This is set up to use a more streamlined look than previous versions and takes inspiration from the Markdown example at
			// https://gist.github.com/Almeeida/41a664d8d5f3a8855591c2f1e0e07b19
			yield return $"```md\n{string.Join("\n", _commands.Commands.Where(x => x.Module.Name != nameof(QuoteModule)).Select(x => x.CustomToString()))}\n```";
			yield return $"```md\n{string.Join("\n", _commands.Commands.Where(x => x.Module.Name == nameof(QuoteModule)).Select(x => x.CustomToString()))}\n```";
		}

		#endregion
	}
}
