using System.Threading.Tasks;
using Discord;

namespace Orabot.Bot.Discord.Logging
{
	public interface ILogEventHandler
	{
		Task Log(LogMessage msg);
	}
}