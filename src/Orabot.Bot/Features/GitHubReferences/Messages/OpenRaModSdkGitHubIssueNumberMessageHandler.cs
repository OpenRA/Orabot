using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Orabot.Bot.Features.GitHubReferences.Messages
{
	internal class OpenRaModSdkGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner => "OpenRA";

		protected override string RepositoryName => "OpenRAModSDK";

		protected override Dictionary<string, int> MinimumHandledNumberPerKeyword { get; } = new Dictionary<string, int>
		{
			{ "sdk", 0 }
		};

		public OpenRaModSdkGitHubIssueNumberMessageHandler(IRestClient restClient, IConfiguration configuration) : base(restClient, configuration) { }
	}
}
