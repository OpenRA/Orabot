using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Bot.Discord.Interactions
{
	public interface ISlashCommandEventHandler
	{
		Task HandleSlashCommandAsync(SocketSlashCommand command);
	}
}
