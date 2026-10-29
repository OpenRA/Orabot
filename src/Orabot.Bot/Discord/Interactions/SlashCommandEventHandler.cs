using Discord.WebSocket;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Orabot.Bot.Discord.Interactions
{
	public class SlashCommandEventHandler : ISlashCommandEventHandler
	{
		private readonly IEnumerable<ISlashCommandHandler> _slashCommandHandlers;

		public SlashCommandEventHandler(IServiceProvider serviceProvider)
		{
			_slashCommandHandlers = serviceProvider.GetServices<ISlashCommandHandler>();
		}

		public async Task HandleSlashCommandAsync(SocketSlashCommand command)
		{
			Parallel.ForEach(_slashCommandHandlers, customMessageHandler =>
			{
				if (customMessageHandler.CanHandle(command))
				{
					customMessageHandler.InvokeAsync(command);
				}
			});
		}
	}
}
