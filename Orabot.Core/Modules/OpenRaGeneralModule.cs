using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Orabot.Core.Modules
{
	public class OpenRaGeneralModule : ModuleBase<SocketCommandContext>
	{
		[Command("supportdir", true)]
		[Summary("Prints information about the game's support directory with default paths on each supported OS.")]
		public async Task SupportDir()
		{
			await ReplyAsync("```" +
							 "The support directory is where the game keeps the saved settings, maps, replays, logs and mod assets.  The default location is as follows, but one can choose to move it to an arbitrary location by passing an Engine.SupportDir argument to the Game.exe\n\n" +
							 "Windows:      %APPDATA%\\OpenRA\\\n" +
							 "macOS:        ~/Library/Application Support/OpenRA/\n" +
							 "GNU/Linux:    $XDG_CONFIG_HOME/openra/openra\n\n" +
							 "Older releases (before playtest-20190825) used different locations, which newer versions may continue to use in some circumstances:\n\n" +
							 "Windows:      %USERPROFILE%\\Documents\\OpenRA\\\n" +
			                 "GNU/Linux:    ~/.openra/\n" +
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

		[Command("sdk")]
		[Summary("Prints information about the OpenRA ModSDK.")]
		public async Task Sdk()
		{
			const string sdkRepoUrl = "https://github.com/OpenRA/OpenRAModSDK";

			var embedBuilder = new EmbedBuilder
			{
				Description = $"[The OpenRA ModSDK]({sdkRepoUrl}) helps you build your own games using the OpenRA engine."
			};

			await ReplyAsync(string.Empty, false, embedBuilder.Build());
		}

		[Command("utility")]
		[Summary("Prints information about the OpenRA Utility.")]
		public async Task Utility()
		{
			const string utilityWikiUrl = "https://github.com/OpenRA/OpenRA/wiki/Utility";

			var embedBuilder = new EmbedBuilder
			{
				Description = $"The OpenRA [Utility]({utilityWikiUrl}) is an executable that ships with the engine.\n" +
				              $"It is run through the command line to perform a wide range of operations like manipulating game assets " +
				              $"and checking mod YAML files for errors."
			};

			await ReplyAsync(string.Empty, false, embedBuilder.Build());
		}

		[Command("orabot")]
		[Summary("Prints information about the OpenRA Utility.")]
		public async Task Orabot()
		{
			const string orabotRepoUrl = "https://github.com/OpenRA/Orabot";

			var embedBuilder = new EmbedBuilder
			{
				Description = $"[Orabot]({orabotRepoUrl}) is a modern successor to the famous OpenRA companion - the IRC orabot."
			};

			await ReplyAsync(string.Empty, false, embedBuilder.Build());
		}
	}
}
