using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Orabot.Core.Abstractions.EventHandlers;
using Orabot.Core.Transformers.AttachmentToMessageTransformers;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.SpecificTextMessageHandlers
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

		public async Task InvokeAsync(SocketUserMessage message)
		{
			if (_attachmentLogFileToMessageTransformer.TryCreateExceptionExplanationMessage(message.Content, out var explanationMessage))
				await message.Channel.SendMessageAsync(explanationMessage);
		}
	}
}
