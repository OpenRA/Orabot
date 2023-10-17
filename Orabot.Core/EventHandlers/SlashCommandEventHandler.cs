using Discord.WebSocket;
using Orabot.Core.Abstractions.EventHandlers;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Orabot.Core.EventHandlers
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
