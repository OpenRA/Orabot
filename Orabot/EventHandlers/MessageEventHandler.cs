using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Orabot.EventHandlers.Abstraction;
using Orabot.EventHandlers.CustomMessageHandlers;

namespace Orabot.EventHandlers
{
	internal class MessageEventHandler : IMessageEventHandler
	{
		private readonly IEnumerable<ICustomMessageHandler> _customMessageHandlers;

		public MessageEventHandler(IServiceProvider serviceProvider)
		{
			_customMessageHandlers = serviceProvider.GetServices<ICustomMessageHandler>();
		}

		public async Task HandleMessageReceivedAsync(SocketMessage messageParam)
		{
			// Don't process the command if it was a System Message or the sender is a bot.
			if (!(messageParam is SocketUserMessage message) || message.Source == MessageSource.Bot)
			{
				return;
			}

			Parallel.ForEach(_customMessageHandlers, customMessageHandler =>
			{
				if (customMessageHandler.CanHandle(message))
				{
					customMessageHandler.Invoke(message);
				}
			});
		}
	}
}
