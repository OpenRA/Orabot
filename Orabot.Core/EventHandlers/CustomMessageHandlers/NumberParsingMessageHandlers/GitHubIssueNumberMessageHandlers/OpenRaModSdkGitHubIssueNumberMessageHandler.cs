using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OpenRaModSdkGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "OpenRAModSDK";

		protected override Dictionary<string, int> MinimumHandledNumberPerKeyword { get; } = new Dictionary<string, int>
		{
			{ "sdk", 0 }
		};

		public OpenRaModSdkGitHubIssueNumberMessageHandler(IRestClient restClient, IConfiguration configuration) : base(restClient, configuration) { }
	}
}
