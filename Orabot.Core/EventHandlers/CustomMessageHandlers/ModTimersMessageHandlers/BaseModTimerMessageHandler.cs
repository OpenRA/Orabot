using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Orabot.Core.Abstractions.EventHandlers;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.ModTimersMessageHandlers
{
	internal class BaseModTimerMessageHandler : ICustomMessageHandler
	{
		private readonly string _regexMatchPattern = "\\b(\\w*[a-zA-Z]\\w*)\\b";
		private readonly string[] _modIdentifiers = { "ts", "ra2" };
		private readonly string[] _keywords = { "when", "status", "release", "public", "available" };

		public bool CanHandle(SocketUserMessage message)
		{
			var words = Regex.Matches(message.Content, _regexMatchPattern).Select(x => x.Value.ToLower()).ToArray();
			return _modIdentifiers.Any(x => words.Contains(x)) && _keywords.Any(x => words.Contains(x)) && message.Content.Contains('?');
		}

		public async Task InvokeAsync(SocketUserMessage message)
		{
			await message.Channel.SendMessageAsync("Timer reset. 7 months remaining.");
		}
	}
}
