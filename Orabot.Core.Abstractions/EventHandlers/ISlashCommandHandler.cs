using Discord.WebSocket;
using System.Threading.Tasks;

namespace Orabot.Core.Abstractions.EventHandlers
{
	public interface ISlashCommandHandler
	{
		bool CanHandle(SocketSlashCommand command);

		Task InvokeAsync(SocketSlashCommand command);
	}
}
