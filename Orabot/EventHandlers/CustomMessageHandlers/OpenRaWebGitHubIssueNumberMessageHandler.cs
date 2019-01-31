namespace Orabot.EventHandlers.CustomMessageHandlers
{
	internal class OpenRaWebGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override int IssueNumberStartPosition { get; } = 4;

		protected override string RegexMatchPattern { get; } = "Web#[0-9]+";

		protected override bool RegexMatchCase { get; } = false;

		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRAWeb";
	}
}
