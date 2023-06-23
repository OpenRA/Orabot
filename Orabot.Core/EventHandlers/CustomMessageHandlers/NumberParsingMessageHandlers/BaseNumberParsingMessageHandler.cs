﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Orabot.Core.Abstractions.EventHandlers;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers
{
	internal abstract class BaseNumberParsingMessageHandler : ICustomMessageHandler
	{
		protected abstract Dictionary<string, int> MinimumHandledNumberPerKeyword { get; }

		protected virtual string[] RegexMatchPatternKeywords => MinimumHandledNumberPerKeyword.Keys.ToArray();

		protected virtual string RegexMatchPattern => "(^|[^a-zA-Z0-9])({keyword}#[0-9]+)";

		protected virtual bool RegexMatchCase => false;

		private readonly RegexOptions _regexOptions;
		private readonly string[] _regexMatchPatterns;

		internal BaseNumberParsingMessageHandler()
		{
			_regexOptions = RegexMatchCase ? RegexOptions.Compiled : RegexOptions.Compiled | RegexOptions.IgnoreCase;
			_regexMatchPatterns = RegexMatchPatternKeywords.Select(x => RegexMatchPattern.Replace("{keyword}", x)).ToArray();
		}

		public bool CanHandle(SocketUserMessage message)
		{
			var canHandle = false;
			var matches = _regexMatchPatterns.SelectMany(regexMatchPattern => Regex.Matches(message.Content, regexMatchPattern, _regexOptions));
			foreach (var match in matches)
			{
				var split = match.Groups.Last().Value.Split('#');
				var keyword = split[0];
				if (int.TryParse(split[1], out var number) && MinimumHandledNumberPerKeyword[keyword] <= number)
				{
					canHandle = true;
				}
			}

			return canHandle;
		}

		public abstract Task InvokeAsync(SocketUserMessage message);

		protected IEnumerable<string> GetMatchedNumbers(string message)
		{
			foreach (var regexMatchPattern in _regexMatchPatterns)
			{
				var matches = Regex.Matches(message, regexMatchPattern, _regexOptions);
				foreach (Match match in matches)
				{
					yield return match.Groups.Last().Value.Split('#')[1];
				}
			}
		}
	}
}
