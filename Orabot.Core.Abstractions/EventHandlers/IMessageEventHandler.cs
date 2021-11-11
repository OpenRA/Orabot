using System.Threading.Tasks;
using Discord.WebSocket;

namespace Orabot.Core.Abstractions.EventHandlers
{
	public interface IMessageEventHandler
	{
		Task HandleMessageReceivedAsync(SocketMessage messageParam);
	}
}