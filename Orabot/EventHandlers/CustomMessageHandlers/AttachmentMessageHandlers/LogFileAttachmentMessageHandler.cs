using System.Linq;
using Discord.WebSocket;
using Orabot.Transformers.AttachmentToMessageTransformers;

namespace Orabot.EventHandlers.CustomMessageHandlers.AttachmentMessageHandlers
{
	internal class LogFileAttachmentMessageHandler : BaseAttachmentMessageHandler
	{
		protected override string FileExtension => ".log";

		private readonly AttachmentLogFileToMessageTransformer _attachmentLogFileToMessageTransformer;

		public LogFileAttachmentMessageHandler(AttachmentLogFileToMessageTransformer attachmentLogFileToMessageTransformer)
		{
			_attachmentLogFileToMessageTransformer = attachmentLogFileToMessageTransformer;
		}

		public override void Invoke(SocketUserMessage message)
		{
			var attachment = message.Attachments.First(x => x.Filename.EndsWith(FileExtension));
			var text = _attachmentLogFileToMessageTransformer.CreateMessage(attachment);
			message.Channel.SendMessageAsync(text);
		}
	}
}
