namespace Orabot.EventHandlers.CustomMessageHandlers
{
	internal class OpenRaWebGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRAWeb";

		protected override string[] RegexMatchPatternKeywords { get; } = { "web" };
	}
}
