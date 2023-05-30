using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orabot.Core.Objects.GitHub;
using RestSharp;

namespace Orabot.Core.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers
{
	internal abstract class BaseGitHubIssueNumberMessageHandler : BaseNumberParsingMessageHandler
	{
		private const string BaseApiUrl = "https://api.github.com";

		private const string ApiIssueRequestTemplate = "repos/{RepositoryOwner}/{RepositoryName}/issues/{number}";

		private const string ApiPullRequestTemplate = "repos/{RepositoryOwner}/{RepositoryName}/pulls/{number}";

		protected abstract string RepositoryOwner { get; }

		protected abstract string RepositoryName { get; }

		private readonly string _issueIconBaseUrl;

		private readonly IRestClient _restClient;
		private readonly Dictionary<string, Color> _colorPerStatus = new Dictionary<string, Color>
		{
			{ "open", Color.Green },
			{ "closed", Color.Red },
			{ "merged", Color.Purple }
		};

		internal BaseGitHubIssueNumberMessageHandler(IRestClient restClient, IConfiguration configuration)
		{
			_restClient = restClient;

			_issueIconBaseUrl = configuration["GitHubIconsBaseUrl"];
		}

		public override async Task InvokeAsync(SocketUserMessage message)
		{
			foreach (var number in GetMatchedNumbers(message.Content))
			{
				var request = new RestRequest($"{BaseApiUrl}/{ApiIssueRequestTemplate}", Method.Get);
				request.AddUrlSegment("RepositoryOwner", RepositoryOwner);
				request.AddUrlSegment("RepositoryName", RepositoryName);
				request.AddUrlSegment("number", number);

				var response = await _restClient.ExecuteAsync<GitHubIssueResponse>(request);
				var issue = response.Data;
				if (issue?.HtmlUrl == null)
				{
					continue;
				}

				var isIssue = issue.PullRequest == null;
				var type = isIssue ? "Issue" : "Pull request";
				var labels = string.Join(", ", issue.Labels?.Select(x => x.Name) ?? Enumerable.Empty<string>());
				var embedFields = new List<EmbedFieldBuilder>();
				if (!string.IsNullOrEmpty(labels))
				{
					embedFields.Add(new EmbedFieldBuilder
					{
						Name = "Labels:",
						Value = labels,
						IsInline = true
					});
				}

				var status = issue.State;

				if (!isIssue && status == "closed")
				{
					var pullRequest = new RestRequest($"{BaseApiUrl}/{ApiPullRequestTemplate}", Method.Get);
					pullRequest.AddUrlSegment("RepositoryOwner", RepositoryOwner);
					pullRequest.AddUrlSegment("RepositoryName", RepositoryName);
					pullRequest.AddUrlSegment("number", number);

					var pullResponse = await _restClient.ExecuteAsync<GitHubPullRequestResponse>(pullRequest);
					var pull = pullResponse.Data;
					if (pull != null)
					{
						embedFields.Add(new EmbedFieldBuilder
						{
							Name = "Status:",
							Value = pull.MergeableState,
							IsInline = true
						});

						status = pull.IsMerged ? "merged" : issue.State;
						if (pull.IsMerged)
						{
							embedFields.Add(new EmbedFieldBuilder
							{
								Name = "Merged by:",
								Value = pull.MergedBy?.LoginName,
								IsInline = true
							});
						}
					}
				}

				var embed = new EmbedBuilder
				{
					Title = issue.Title,
					ThumbnailUrl = issue.User?.AvatarUrl,
					Url = issue.HtmlUrl,
					Description = issue.Body.Length > 250 ? issue.Body.Substring(0, 250) + "..." : issue.Body,
					Author = new EmbedAuthorBuilder
					{
						Name = $"{type} #{number} by {issue.User?.LoginName}  ({status})",
						IconUrl = GetIssueIconUrl(isIssue, status),
						Url = issue.HtmlUrl
					},
					Fields = embedFields,
					Footer = new EmbedFooterBuilder
					{
						Text = $"Created at {issue.CreatedAt}"
					},
					Timestamp = issue.UpdatedAt,
					Color = _colorPerStatus[status]
				};

				await message.Channel.SendMessageAsync("", embed: embed.Build());
			}
		}

		private string GetIssueIconUrl(bool isIssue, string status)
		{
			return $"{_issueIconBaseUrl}/github-{(isIssue ? "issue" : "pr")}-{status}.png";
		}
	}
}
