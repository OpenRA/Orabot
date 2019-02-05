using RestSharp;

namespace Orabot.EventHandlers.CustomMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OpenRaGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRA";

		protected override string[] RegexMatchPatternKeywords { get; } = { string.Empty };

		public OpenRaGitHubIssueNumberMessageHandler(IRestClient restClient) : base(restClient) { }
	}
}
