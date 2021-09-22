using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Orabot.Core.Abstractions.EventHandlers
{
	public interface IReactionEventHandler
	{
		Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> messageGetter, ISocketMessageChannel channel, SocketReaction reaction);

		Task HandleReactionRemovedAsync(Cacheable<IUserMessage, ulong> messageGetter, ISocketMessageChannel channel, SocketReaction reaction);
	}
}
