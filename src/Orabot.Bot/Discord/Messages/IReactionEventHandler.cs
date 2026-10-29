using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Orabot.Bot.Discord.Messages
{
	public interface IReactionEventHandler
	{
		Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> messageGetter, Cacheable<IMessageChannel, ulong> channelGetter, SocketReaction reaction);

		Task HandleReactionRemovedAsync(Cacheable<IUserMessage, ulong> messageGetter, Cacheable<IMessageChannel, ulong> channelGetter, SocketReaction reaction);
	}
}
