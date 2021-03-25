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
			if (string.IsNullOrWhiteSpace(rawMessage))
				return;

			if (_attachmentLogFileToMessageTransformer.TryCreateExceptionExplanationMessage(fullText, out var explanationMessage))
			{
				rawMessage = $"{rawMessage}\r\n{explanationMessage}";
			}

			message.Channel.SendMessageAsync(rawMessage);
		}
	}
}
