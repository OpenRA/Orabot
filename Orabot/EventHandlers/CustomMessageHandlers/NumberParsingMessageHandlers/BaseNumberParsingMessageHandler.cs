using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers
{
	internal abstract class BaseNumberParsingMessageHandler : ICustomMessageHandler
	{
		protected abstract string[] RegexMatchPatternKeywords { get; }

		protected virtual string RegexMatchPattern { get; } = "(^|[^a-zA-Z0-9]){keyword}#[0-9]+";

		protected virtual bool RegexMatchCase { get; } = false;

		private readonly RegexOptions _regexOptions;
		private readonly string[] _regexMatchPatterns;

		internal BaseNumberParsingMessageHandler()
		{
			_regexOptions = RegexMatchCase ? RegexOptions.Compiled : RegexOptions.Compiled | RegexOptions.IgnoreCase;
			_regexMatchPatterns = RegexMatchPatternKeywords.Select(x => RegexMatchPattern.Replace("{keyword}", x)).ToArray();
		}

		public bool CanHandle(SocketUserMessage message)
		{
			return _regexMatchPatterns.Any(regexMatchPattern => Regex.IsMatch(message.Content, regexMatchPattern, _regexOptions));
		}

		public abstract void Invoke(SocketUserMessage message);

		protected IEnumerable<string> GetMatchedNumbers(string message)
		{
			foreach (var regexMatchPattern in _regexMatchPatterns)
			{
				var matches = Regex.Matches(message, regexMatchPattern, _regexOptions);
				foreach (Match match in matches)
				{
					yield return match.Value.Substring(match.Value.LastIndexOf('#') + 1);
				}
			}
		}
	}
}
