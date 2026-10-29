using System.Threading.Tasks;
using Discord.WebSocket;

namespace Orabot.Bot.Discord.Messages
{
	public interface IMessageEventHandler
	{
		Task HandleMessageReceivedAsync(SocketMessage messageParam);
	}
}