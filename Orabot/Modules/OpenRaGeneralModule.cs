using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Orabot.Modules
{
	public class OpenRaGeneralModule : ModuleBase<SocketCommandContext>
	{
		[Command("supportdir", true)]
		[Summary("Gives information about the game's support directory with default paths on each supported OS.")]
		public async Task SupportDir()
		{
			await ReplyAsync("```" +
							 "The support directory is where the game keeps the saved settings, maps, replays, logs and mod assets.  The default location is as follows, but one can choose to move it to an arbitrary location by passing an Engine.SupportDir argument to the Game.exe\n\n" +
			                 "Windows:    \\Users\\<Username>\\My Documents\\OpenRA\\\n" +
			                 "OS X:        /Users/<username>/Library/Application Support/OpenRA/\n" +
			                 "GNU/Linux:    /home/<username>/.openra/\n" +
			                 "```");
		}

		[Command("academy")]
		[Summary("Prints information about the OpenRA Academy.")]
		public async Task Academy()
		{
			const string messageLink = "https://discordapp.com/channels/153649279762694144/520193572256088084/520209549274120202";

			if (!(Context.Guild.Channels.FirstOrDefault(x => x.Name == "navigation") is ITextChannel channel))
			{
				return;
			}

			var embedBuilder = new EmbedBuilder
			{
				Description = "The OpenRA Academy is a separate Discord server aimed at helping players get better at the game.\n" +
				              $"Please refer to [the following message]({messageLink}) in {channel.Mention}, which contains a link with an invite to the server."
			};

			await ReplyAsync(string.Empty, false, embedBuilder.Build());
		}

		[Command("book")]
		[Summary("Prints information about the OpenRA Book.")]
		public async Task Book()
		{
			const string bookUrl = "https://www.openra.net/book/";
			const string bookRepoUrl = "https://github.com/OpenRA/book";

			var embedBuilder = new EmbedBuilder
			{
				Description = $"[The OpenRA book]({bookUrl}) aims to teach new players how to play and existing players how to get better.\n" +
				              "It also covers creating new games on the OpenRA engine and how the engine works under-the-hood.\n" +
				              $"You can find the official repository for the book [on GitHub]({bookRepoUrl})."
			};

			await ReplyAsync(string.Empty, false, embedBuilder.Build());
		}
	}
}
