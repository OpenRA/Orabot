using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Orabot.Bot.Discord.Messages
{
	internal abstract class BaseAttachmentMessageHandler : ICustomMessageHandler
	{
		protected abstract string FileExtension { get; }

		public virtual bool CanHandle(SocketUserMessage message)
		{
			return message.Attachments.Count != 0 && message.Attachments.Any(x => x.Filename.EndsWith(FileExtension));
		}

		public abstract Task InvokeAsync(SocketUserMessage message);
	}
}
