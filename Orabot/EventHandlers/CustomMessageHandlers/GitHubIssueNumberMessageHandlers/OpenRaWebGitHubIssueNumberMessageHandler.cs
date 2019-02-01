using System;

namespace Orabot.EventHandlers.CustomMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OpenRaWebGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRAWeb";

		protected override string[] RegexMatchPatternKeywords { get; } = { "web" };

		public OpenRaWebGitHubIssueNumberMessageHandler(IServiceProvider serviceProvider) : base(serviceProvider) { }
	}
}
