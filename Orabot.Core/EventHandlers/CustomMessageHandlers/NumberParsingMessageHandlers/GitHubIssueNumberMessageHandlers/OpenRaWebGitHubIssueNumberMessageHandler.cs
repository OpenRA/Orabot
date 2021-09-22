using System.Collections.Generic;
using RestSharp;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OpenRaWebGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRAWeb";

		protected override Dictionary<string, int> MinimumHandledNumberPerKeyword { get; } = new Dictionary<string, int>
		{
			{ "web", 0 }
		};

		public OpenRaWebGitHubIssueNumberMessageHandler(IRestClient restClient) : base(restClient) { }
	}
}
