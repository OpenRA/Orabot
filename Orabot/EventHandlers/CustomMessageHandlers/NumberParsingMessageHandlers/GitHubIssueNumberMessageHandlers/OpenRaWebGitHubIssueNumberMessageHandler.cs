using RestSharp;

namespace Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OpenRaWebGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRAWeb";

		protected override string[] RegexMatchPatternKeywords { get; } = { "web" };

		public OpenRaWebGitHubIssueNumberMessageHandler(IRestClient restClient) : base(restClient) { }
	}
}
