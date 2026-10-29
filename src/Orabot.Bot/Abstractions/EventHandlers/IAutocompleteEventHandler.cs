using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Core.Abstractions.EventHandlers
{
	public interface IAutocompleteEventHandler
	{
		Task HandleAutocompleteAsync(SocketAutocompleteInteraction interaction);
	}
}
