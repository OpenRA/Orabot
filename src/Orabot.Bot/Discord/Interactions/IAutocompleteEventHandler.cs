using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Bot.Discord.Interactions
{
	public interface IAutocompleteEventHandler
	{
		Task HandleAutocompleteAsync(SocketAutocompleteInteraction interaction);
	}
}
