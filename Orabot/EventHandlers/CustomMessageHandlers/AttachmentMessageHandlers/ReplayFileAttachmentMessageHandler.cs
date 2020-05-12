using System.Linq;
using Discord.WebSocket;
using Orabot.Transformers.Replays.ReplayDataToEmbedTransformers;
using Orabot.Transformers.Replays.ReplayToReplayDataTransformers;

namespace Orabot.EventHandlers.CustomMessageHandlers.AttachmentMessageHandlers
{
	internal class ReplayFileAttachmentMessageHandler : BaseAttachmentMessageHandler
	{
		private const string ReplayFileExtension = ".orarep";

		private readonly AttachmentReplayToUtilityMetadataTransformer _toUtilityMetadataTransformer;
		private readonly UtilityReplayMetadataToEmbedTransformer _toEmbedTransformer;

		public ReplayFileAttachmentMessageHandler(AttachmentReplayToUtilityMetadataTransformer toUtilityMetadataTransformer, UtilityReplayMetadataToEmbedTransformer toEmbedTransformer)
		{
			_toUtilityMetadataTransformer = toUtilityMetadataTransformer;
			_toEmbedTransformer = toEmbedTransformer;
		}

		public override bool CanHandle(SocketUserMessage message)
		{
			return message.Attachments.Count != 0 && message.Attachments.Any(x => x.Filename.EndsWith(ReplayFileExtension));
		}

		public override void Invoke(SocketUserMessage message)
		{
			var attachment = message.Attachments.First(x => x.Filename.EndsWith(ReplayFileExtension));

			var replayMetadata = _toUtilityMetadataTransformer.GetMetadata(attachment);
			replayMetadata.FileName = attachment.Filename;
			var embed = _toEmbedTransformer.CreateEmbed(replayMetadata, attachment.Url);
			if (embed != null)
			{
				var replyMessage = $"{message.Author.Mention} posted a replay";
				if (string.IsNullOrWhiteSpace(message.Content))
				{
					replyMessage += ":";
				}
				else
				{
					replyMessage += $" with commentary:\n{message.Content}";
				}

				message.Channel.SendMessageAsync(replyMessage, embed: embed);
			}
		}
	}
}
