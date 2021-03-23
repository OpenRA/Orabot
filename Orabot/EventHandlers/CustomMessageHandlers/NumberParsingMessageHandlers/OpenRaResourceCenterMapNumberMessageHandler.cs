using System.Collections.Generic;
using Discord.WebSocket;
using Orabot.Transformers.LinkToEmbedTransformers;

namespace Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers
{
	internal class OpenRaResourceCenterMapNumberMessageHandler : BaseNumberParsingMessageHandler
	{
		protected override Dictionary<string, int> MinimumHandledNumberPerKeyword { get; } = new Dictionary<string, int>
		{
			{ "map", 0 }
		};

		private readonly OpenRaResourceCenterMapLinkToEmbedTransformer _toEmbedTransformer;

		public OpenRaResourceCenterMapNumberMessageHandler(OpenRaResourceCenterMapLinkToEmbedTransformer toEmbedTransformer)
		{
			_toEmbedTransformer = toEmbedTransformer;
		}

		public override void Invoke(SocketUserMessage message)
		{
			foreach (var numberStr in GetMatchedNumbers(message.Content))
			{
				if (!int.TryParse(numberStr, out var number))
				{
					continue;
				}

				var embed = _toEmbedTransformer.CreateEmbed(number);
				if (embed != null)
				{
					message.Channel.SendMessageAsync("", embed: embed);
				}
			}
		}
	}
}
