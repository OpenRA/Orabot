using Discord.WebSocket;

namespace Orabot.EventHandlers.CustomMessageHandlers
{
	internal interface ICustomMessageHandler
	{
		string HandlingCategory { get; }

		int HandlingPriority { get; }

		bool CanHandle(SocketUserMessage message);

		void Invoke(SocketUserMessage message);
	}
}
