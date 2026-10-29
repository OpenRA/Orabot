using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Orabot.Bot.Features.GitHubReferences.Messages
{
	internal class OrabotGitHubIssueNumberMessageHandler : BaseGitHubIssueNumberMessageHandler
	{
		protected override string RepositoryOwner => "OpenRA";

		protected override string RepositoryName => "Orabot";

		protected override Dictionary<string, int> MinimumHandledNumberPerKeyword { get; } = new Dictionary<string, int>
		{
			{ "bot", 0 }
		};

		public OrabotGitHubIssueNumberMessageHandler(IRestClient restClient, IConfiguration configuration) : base(restClient, configuration) { }
	}
}
