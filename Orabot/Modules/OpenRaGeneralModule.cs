using System.Threading.Tasks;
using Discord.Commands;

namespace Orabot.Modules
{
	public class OpenRaGeneralModule : ModuleBase<SocketCommandContext>
	{
		[Command("supportdir", true)]
		public async Task SupportDir()
		{
			await ReplyAsync("```" +
							 "The support directory is where the game keeps the saved settings, maps, replays, logs and mod assets.  The default location is as follows, but one can choose to move it to an arbitrary location by passing an Engine.SupportDir argument to the Game.exe\n\n" +
			                 "Windows:    \\Users\\<Username>\\My Documents\\OpenRA\\\n" +
			                 "OS X:        /Users/<username>/Library/Application Support/OpenRA/\n" +
			                 "GNU/Linux:    /home/<username>/.openra/\n" +
			                 "```");
		}
	}
}
