using System.Threading.Tasks;
using Discord;

namespace Orabot.EventHandlers.Abstraction
{
	public interface ILogEventHandler
	{
		Task Log(LogMessage msg);
	}
}