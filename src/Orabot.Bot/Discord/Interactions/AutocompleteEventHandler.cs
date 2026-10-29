using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orabot.Bot.Discord.Interactions
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
			if (interaction.HasResponded || interaction.Type != global::Discord.InteractionType.ApplicationCommandAutocomplete)
				return;
			
			Parallel.ForEach(_autocompleteHandlers, handler =>
			{
				if (handler.CanHandle(interaction))
					handler.InvokeAsync(interaction);
			});
		}
	}
}
