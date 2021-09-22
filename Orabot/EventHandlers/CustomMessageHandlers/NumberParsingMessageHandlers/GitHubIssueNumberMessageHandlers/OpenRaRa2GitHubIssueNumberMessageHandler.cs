using System.Collections.Generic;
using RestSharp;

namespace Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OpenRaRa2GitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "ra2";

		protected override Dictionary<string, int> MinimumHandledNumberPerKeyword { get; } = new Dictionary<string, int>
		{
			{ "ra2", 0 }
		};

		public OpenRaRa2GitHubIssueNumberMessageHandler(IRestClient restClient) : base(restClient) { }
	}
}
