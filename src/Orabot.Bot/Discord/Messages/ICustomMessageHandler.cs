using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Bot.Discord.Messages
{
	public interface ICustomMessageHandler
	{
		bool CanHandle(SocketUserMessage message);

		Task InvokeAsync(SocketUserMessage message);
	}
}
