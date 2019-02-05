using Discord.WebSocket;
using Orabot.Transformers.LinkToEmbedTransformers;

namespace Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers
{
	internal class OpenRaResourceCenterMapNumberMessageHandler : BaseNumberParsingMessageHandler
	{
		protected override string[] RegexMatchPatternKeywords { get; } = { "map" };

		private readonly OpenRaResourceCenterMapLinkToEmbedTransformer _toEmbedTransformer;

		public OpenRaResourceCenterMapNumberMessageHandler(OpenRaResourceCenterMapLinkToEmbedTransformer toEmbedTransformer)
		{
			_toEmbedTransformer = toEmbedTransformer;
		}

		public override void Invoke(SocketUserMessage message)
		{
			foreach (var number in GetMatchedNumbers(message.Content))
			{
				var embed = _toEmbedTransformer.CreateEmbed(number);
				if (embed != null)
				{
					message.Channel.SendMessageAsync("", embed: embed);
				}
			}
		}
	}
}
