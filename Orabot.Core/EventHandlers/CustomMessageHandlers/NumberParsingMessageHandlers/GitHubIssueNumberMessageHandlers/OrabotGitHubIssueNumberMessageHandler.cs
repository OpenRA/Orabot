using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal class OrabotGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner { get; } = "OpenRA";

		protected override string RepositoryName { get; } = "Orabot";

		protected override Dictionary<string, int> MinimumHandledNumberPerKeyword { get; } = new Dictionary<string, int>
		{
			{ "bot", 0 }
		};

		public OrabotGitHubIssueNumberMessageHandler(IRestClient restClient, IConfiguration configuration) : base(restClient, configuration) { }
	}
}
