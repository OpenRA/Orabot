using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Orabot.Core.Abstractions.EventHandlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orabot.Core.EventHandlers
{
	public class AutocompleteEventHandler : IAutocompleteEventHandler
	{
		private readonly IEnumerable<IAutocompleteHandler> _autocompleteHandlers;

		public AutocompleteEventHandler(IServiceProvider serviceProvider)
		{
			_autocompleteHandlers = serviceProvider.GetServices<IAutocompleteHandler>();
		}

		public async Task HandleAutocompleteAsync(SocketAutocompleteInteraction interaction)
		{
			if (interaction.HasResponded || interaction.Type != Discord.InteractionType.ApplicationCommandAutocomplete)
				return;
			
			Parallel.ForEach(_autocompleteHandlers, handler =>
			{
				if (handler.CanHandle(interaction))
					handler.InvokeAsync(interaction);
			});
		}
	}
}
