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
			var rawMessage = _attachmentLogFileToMessageTransformer.CreateRawMessage(attachment, out var fullText);
			message.Channel.SendMessageAsync(rawMessage);

			if (_attachmentLogFileToMessageTransformer.TryCreateExceptionExplanationMessage(fullText, out var explanationMessage))
			{
				message.Channel.SendMessageAsync(explanationMessage);
			}
		}
	}
}
