using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Core.Abstractions.EventHandlers
{
	public interface IAutocompleteHandler
	{
		bool CanHandle(SocketAutocompleteInteraction interaction);

		Task InvokeAsync(SocketAutocompleteInteraction interaction);
	}
}
