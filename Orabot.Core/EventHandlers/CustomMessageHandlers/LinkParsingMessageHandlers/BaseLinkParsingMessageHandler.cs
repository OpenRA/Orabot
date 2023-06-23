﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Orabot.Core.Abstractions.EventHandlers;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.LinkParsingMessageHandlers
{
	internal abstract class BaseLinkParsingMessageHandler : ICustomMessageHandler
	{
		protected abstract string[] RegexMatchPatterns { get; }

		protected virtual bool RegexMatchCase { get; } = false;

		private readonly RegexOptions _regexOptions;

		internal BaseLinkParsingMessageHandler()
		{
			_regexOptions = RegexMatchCase ? RegexOptions.Compiled : RegexOptions.Compiled | RegexOptions.IgnoreCase;
		}

		public bool CanHandle(SocketUserMessage message)
		{
			return RegexMatchPatterns.Any(regexMatchPattern => Regex.IsMatch(message.Content, regexMatchPattern, _regexOptions));
		}

		public abstract Task InvokeAsync(SocketUserMessage message);

		protected IEnumerable<string> GetMatchedLinks(string message)
		{
			foreach (var regexMatchPattern in RegexMatchPatterns)
			{
				var matches = Regex.Matches(message, regexMatchPattern, _regexOptions);
				foreach (Match match in matches)
				{
					yield return match.Value;
				}
			}
		}
	}
}
