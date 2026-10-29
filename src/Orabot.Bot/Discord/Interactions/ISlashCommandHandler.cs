using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Bot.Discord.Interactions
{
	public interface ISlashCommandHandler
	{
		bool CanHandle(SocketSlashCommand command);

		Task InvokeAsync(SocketSlashCommand command);
	}
}
