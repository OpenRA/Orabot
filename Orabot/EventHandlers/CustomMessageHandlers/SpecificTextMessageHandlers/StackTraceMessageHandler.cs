using System.Text.RegularExpressions;
using Discord.WebSocket;
using Orabot.Transformers.AttachmentToMessageTransformers;

namespace Orabot.EventHandlers.CustomMessageHandlers.SpecificTextMessageHandlers
{
	internal class StackTraceMessageHandler : ICustomMessageHandler
	{
		private readonly AttachmentLogFileToMessageTransformer _attachmentLogFileToMessageTransformer;

		public StackTraceMessageHandler(AttachmentLogFileToMessageTransformer attachmentLogFileToMessageTransformer)
		{
			_attachmentLogFileToMessageTransformer = attachmentLogFileToMessageTransformer;
		}

		public bool CanHandle(SocketUserMessage message)
		{
			return Regex.IsMatch(message.Content, "Exception of type `[a-zA-Z.]*`: ", RegexOptions.Compiled)
				|| Regex.IsMatch(message.Content, "Exception of type [a-zA-Z.]*: ", RegexOptions.Compiled);
		}

		public void Invoke(SocketUserMessage message)
		{
			if (_attachmentLogFileToMessageTransformer.TryCreateExceptionExplanationMessage(message.Content, out var explanationMessage))
				message.Channel.SendMessageAsync(explanationMessage);
		}
	}
}
