using Discord.WebSocket;

namespace Orabot.EventHandlers.CustomMessageHandlers
{
	internal interface ICustomMessageHandler
	{
		bool CanHandle(SocketUserMessage message);

		void Invoke(SocketUserMessage message);
	}
}
