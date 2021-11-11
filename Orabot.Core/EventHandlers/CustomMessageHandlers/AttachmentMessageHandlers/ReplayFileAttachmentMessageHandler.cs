using System.Linq;
using Discord.WebSocket;
using Orabot.Core.Transformers.Replays.ReplayDataToEmbedTransformers;
using Orabot.Core.Transformers.Replays.ReplayToReplayDataTransformers;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.AttachmentMessageHandlers
{
	internal class ReplayFileAttachmentMessageHandler : BaseAttachmentMessageHandler
	{
		protected override string FileExtension => ".orarep";

		private readonly AttachmentReplayToUtilityMetadataTransformer _toUtilityMetadataTransformer;
		private readonly UtilityReplayMetadataToEmbedTransformer _toEmbedTransformer;

		public ReplayFileAttachmentMessageHandler(AttachmentReplayToUtilityMetadataTransformer toUtilityMetadataTransformer, UtilityReplayMetadataToEmbedTransformer toEmbedTransformer)
		{
			_toUtilityMetadataTransformer = toUtilityMetadataTransformer;
			_toEmbedTransformer = toEmbedTransformer;
		}

		public override void Invoke(SocketUserMessage message)
		{
			var attachment = message.Attachments.First(x => x.Filename.EndsWith(FileExtension));

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
