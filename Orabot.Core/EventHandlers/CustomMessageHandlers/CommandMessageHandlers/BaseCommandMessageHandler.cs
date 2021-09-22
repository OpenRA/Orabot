using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Orabot.Core.Abstractions.EventHandlers;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.CommandMessageHandlers
{
	internal class BaseCommandMessageHandler : ICustomMessageHandler
	{
		private const char CommandChar = ']';

		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly IServiceProvider _serviceProvider;

		public BaseCommandMessageHandler(IServiceProvider serviceProvider)
		{
			_client = serviceProvider.GetRequiredService<DiscordSocketClient>();
			_commands = serviceProvider.GetRequiredService<CommandService>();
			_serviceProvider = serviceProvider;
		}

		public bool CanHandle(SocketUserMessage message)
		{
			var argumentPosition = 0;
			return message.HasCharPrefix(CommandChar, ref argumentPosition);
		}

		public void Invoke(SocketUserMessage message)
		{
			var argumentPosition = 0;
			message.HasCharPrefix(CommandChar, ref argumentPosition);

			var context = new SocketCommandContext(_client, message);
			var result = _commands.ExecuteAsync(context, argumentPosition, _serviceProvider).Result;
			if (!result.IsSuccess)
			{
				context.Channel.SendMessageAsync(result.ErrorReason);
			}
		}
	}
}
