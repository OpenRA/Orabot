﻿using Discord.WebSocket;
using Orabot.Core.Transformers.LinkToEmbedTransformers;
using System.Threading.Tasks;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.LinkParsingMessageHandlers
{
	internal class OpenRaResourceCenterMapLinkMessageHandler : BaseLinkParsingMessageHandler
	{
		protected override string[] RegexMatchPatterns { get; } = { "https://resource.openra.net/maps/[0-9]+" };

		private readonly OpenRaResourceCenterMapLinkToEmbedTransformer _toEmbedTransformer;

		public OpenRaResourceCenterMapLinkMessageHandler(OpenRaResourceCenterMapLinkToEmbedTransformer toEmbedTransformer)
		{
			_toEmbedTransformer = toEmbedTransformer;
		}

		public override async Task InvokeAsync(SocketUserMessage message)
		{
			foreach (var matchedLink in GetMatchedLinks(message.Content))
			{
				var numberStr = matchedLink.Substring(matchedLink.LastIndexOf('/') + 1);
				if (!int.TryParse(numberStr, out var number))
				{
					continue;
				}

				var embed = await _toEmbedTransformer.CreateEmbed(number);
				if (embed != null)
					await message.Channel.SendMessageAsync("", embed: embed);
			}
		}
	}
}
