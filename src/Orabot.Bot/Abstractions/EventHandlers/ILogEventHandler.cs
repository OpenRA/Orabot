using System.Threading.Tasks;
using Discord;

namespace Orabot.Core.Abstractions.EventHandlers
{
	public interface ILogEventHandler
	{
		Task Log(LogMessage msg);
	}
}