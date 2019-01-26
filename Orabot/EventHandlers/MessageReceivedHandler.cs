using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Orabot.EventHandlers.CustomMessageHandlers;

namespace Orabot.EventHandlers
{
	internal static class MessageReceivedHandler
	{
		private static readonly IReadOnlyDictionary<string, ReadOnlyCollection<ICustomMessageHandler>> CustomMessageHandlersPerCategory = LoadMessageHandlers();

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

			Parallel.ForEach(CustomMessageHandlersPerCategory, customMessageHandlerCategory =>
			{
				foreach (var customMessageHandler in customMessageHandlerCategory.Value)
				{
					if (customMessageHandler.CanHandle(message))
					{
						customMessageHandler.Invoke(message);
						break;
					}
				}
			});

			return Task.CompletedTask;
		}

		private static IReadOnlyDictionary<string, ReadOnlyCollection<ICustomMessageHandler>> LoadMessageHandlers()
		{
			return new ReadOnlyDictionary<string, ReadOnlyCollection<ICustomMessageHandler>>(
				AppDomain.CurrentDomain
					.GetAssemblies()
					.SelectMany(x => x.GetTypes().Where(y => !y.IsAbstract && y.GetInterfaces().Contains(typeof(ICustomMessageHandler))))
					.Select(x => (ICustomMessageHandler)Activator.CreateInstance(x))
					.GroupBy(x => x.HandlingCategory, y => y)
					.ToDictionary(x => x.Key, y => y.OrderBy(z => z.HandlingPriority).ToList().AsReadOnly()));
		}
	}
}
