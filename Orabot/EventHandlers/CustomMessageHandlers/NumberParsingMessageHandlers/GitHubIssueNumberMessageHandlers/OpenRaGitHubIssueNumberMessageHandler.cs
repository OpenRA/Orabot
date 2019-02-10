using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OpenRaGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRA";

		protected override Dictionary<string, int> MinimumHandledNumberPerKeyword { get; } = new Dictionary<string, int>
		{
			{ string.Empty, 2000 },
			{ "ora", 0 }
		};

		public OpenRaGitHubIssueNumberMessageHandler(IRestClient restClient) : base(restClient) { }
	}
}
