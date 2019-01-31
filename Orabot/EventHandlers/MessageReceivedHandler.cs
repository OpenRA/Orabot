using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Orabot.EventHandlers.CustomMessageHandlers;

namespace Orabot.EventHandlers
{
	internal static class MessageReceivedHandler
	{
		private static readonly IEnumerable<ICustomMessageHandler> CustomMessageHandlers = LoadMessageHandlers();

		public static Task HandleMessageReceivedAsync(SocketMessage messageParam)
		{
			// Don't process the command if it was a System Message
			if (!(messageParam is SocketUserMessage message))
			{
				return Task.CompletedTask;
			}

			if (message.Source == MessageSource.Bot)
			{
				return Task.CompletedTask;
			}

			Parallel.ForEach(CustomMessageHandlers, customMessageHandler =>
			{
				if (customMessageHandler.CanHandle(message))
				{
					customMessageHandler.Invoke(message);
				}
			});

			return Task.CompletedTask;
		}

		private static IEnumerable<ICustomMessageHandler> LoadMessageHandlers()
		{
			return AppDomain.CurrentDomain
					.GetAssemblies()
					.SelectMany(x => x.GetTypes().Where(y => !y.IsAbstract && y.GetInterfaces().Contains(typeof(ICustomMessageHandler))))
					.Select(x => (ICustomMessageHandler)Activator.CreateInstance(x));
		}
	}
}
