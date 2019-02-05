using Discord.WebSocket;
using Orabot.Transformers.LinkToEmbedTransformers;

namespace Orabot.EventHandlers.CustomMessageHandlers.LinkParsingMessageHandlers
{
	internal class OpenRaResourceCenterMapLinkMessageHandler : BaseLinkParsingMessageHandler
	{
		protected override string[] RegexMatchPatterns { get; } = { "https://resource.openra.net/maps/[0-9]+" };

		private readonly OpenRaResourceCenterMapLinkToEmbedTransformer _toEmbedTransformer;

		public OpenRaResourceCenterMapLinkMessageHandler(OpenRaResourceCenterMapLinkToEmbedTransformer toEmbedTransformer)
		{
			_toEmbedTransformer = toEmbedTransformer;
		}

		public override void Invoke(SocketUserMessage message)
		{
			foreach (var matchedLink in GetMatchedLinks(message.Content))
			{
				var number = matchedLink.Substring(matchedLink.LastIndexOf('/') + 1);
				var embed = _toEmbedTransformer.CreateEmbed(number);
				if (embed != null)
				{
					message.Channel.SendMessageAsync("", embed: embed);
				}
			}
		}
	}
}
