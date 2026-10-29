using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Core.Abstractions.EventHandlers
{
	public interface ICustomMessageHandler
	{
		bool CanHandle(SocketUserMessage message);

		Task InvokeAsync(SocketUserMessage message);
	}
}
