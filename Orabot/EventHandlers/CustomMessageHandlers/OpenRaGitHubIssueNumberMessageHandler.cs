namespace Orabot.EventHandlers.CustomMessageHandlers
{
	internal class OpenRaGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override int IssueNumberStartPosition { get; } = 1;

		protected override string RegexMatchPattern { get; } = "#[0-9]+";

		protected override bool RegexMatchCase { get; } = false;

		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRA";

		public override int HandlingPriority { get; } = 20;
	}
}
