using RestSharp;

namespace Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OpenRaModSdkGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRAModSDK";

		protected override string[] RegexMatchPatternKeywords { get; } = { "sdk" };

		public OpenRaModSdkGitHubIssueNumberMessageHandler(IRestClient restClient) : base(restClient) { }
	}
}
