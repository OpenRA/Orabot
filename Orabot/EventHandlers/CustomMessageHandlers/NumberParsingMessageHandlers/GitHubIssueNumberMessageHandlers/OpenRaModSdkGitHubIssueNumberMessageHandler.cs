using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OpenRaModSdkGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRAModSDK";

		protected override Dictionary<string, int> MinimumHandledNumberPerKeyword { get; } = new Dictionary<string, int>
		{
			{ "sdk", 0 }
		};

		public OpenRaModSdkGitHubIssueNumberMessageHandler(IRestClient restClient) : base(restClient) { }
	}
}
