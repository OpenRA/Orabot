using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Core.Abstractions.EventHandlers
{
	public interface ISlashCommandEventHandler
	{
		Task HandleSlashCommandAsync(SocketSlashCommand command);
	}
}
