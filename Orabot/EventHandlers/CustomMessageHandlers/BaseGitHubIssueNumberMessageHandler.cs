using System;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace Orabot.EventHandlers.CustomMessageHandlers
{
	internal abstract class BaseGitHubIssueNumberMessageHandler : ICustomMessageHandler
	{
		protected abstract int IssueNumberStartPosition { get; }

		protected abstract string RegexMatchPattern { get; }

		protected abstract string RepositoryOwner { get; }

		protected abstract string RepositoryName { get; }

		public string HandlingCategory { get; } = nameof(BaseGitHubIssueNumberMessageHandler);

		public abstract int HandlingPriority { get; }

		private const string BaseApiUrl = "https://api.github.com/repos";

		public bool CanHandle(SocketUserMessage message)
		{
			return Regex.IsMatch(message.Content, RegexMatchPattern);
		}

		public void Invoke(SocketUserMessage message)
		{
			var matches = Regex.Matches(message.Content, RegexMatchPattern, RegexOptions.Compiled);
			foreach (Match match in matches)
			{
				var number = match.Value.Substring(IssueNumberStartPosition);
				var url = $"{BaseApiUrl}/{RepositoryOwner}/{RepositoryName}/issues/{number}";

				Console.WriteLine(url);
			}
		}
	}
}
