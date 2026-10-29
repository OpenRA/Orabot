using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Bot.Discord.Interactions
{
	public interface IAutocompleteHandler
	{
		bool CanHandle(SocketAutocompleteInteraction interaction);

		Task InvokeAsync(SocketAutocompleteInteraction interaction);
	}
}
