using System.Threading.Tasks;
using Discord.WebSocket;

namespace Orabot.EventHandlers.Abstraction
{
	public interface IMessageEventHandler
	{
		Task HandleMessageReceivedAsync(SocketMessage messageParam);
	}
}