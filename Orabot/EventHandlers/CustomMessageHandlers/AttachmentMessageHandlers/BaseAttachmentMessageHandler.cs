using Discord.WebSocket;

namespace Orabot.EventHandlers.CustomMessageHandlers.AttachmentMessageHandlers
{
	internal abstract class BaseAttachmentMessageHandler : ICustomMessageHandler
	{
		public abstract bool CanHandle(SocketUserMessage message);

		public abstract void Invoke(SocketUserMessage message);
	}
}
