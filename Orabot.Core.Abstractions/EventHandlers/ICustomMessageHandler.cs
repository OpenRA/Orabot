using Discord.WebSocket;

namespace Orabot.Core.Abstractions.EventHandlers
{
	public interface ICustomMessageHandler
	{
		bool CanHandle(SocketUserMessage message);

		void Invoke(SocketUserMessage message);
	}
}
