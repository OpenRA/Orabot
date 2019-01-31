using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Orabot.Objects.GitHub;
using RestSharp;

namespace Orabot.EventHandlers.CustomMessageHandlers
{
	internal abstract class BaseGitHubIssueNumberMessageHandler : ICustomMessageHandler
	{
		private static readonly string IssueIconBaseUrl = ConfigurationManager.AppSettings["GitHubIconsBaseUrl"];

		protected abstract int IssueNumberStartPosition { get; }

		protected abstract string RegexMatchPattern { get; }

		protected abstract bool RegexMatchCase { get; }

		protected abstract string RepositoryOwner { get; }

		protected abstract string RepositoryName { get; }

		private const string BaseApiUrl = "https://api.github.com";

		private const string ApiIssueRequestTemplate = "repos/{RepositoryOwner}/{RepositoryName}/issues/{number}";

		private const string ApiPullRequestTemplate = "repos/{RepositoryOwner}/{RepositoryName}/pulls/{number}";

		private readonly RestClient _restClient = new RestClient(BaseApiUrl);
		private readonly Dictionary<string, Color> _colorPerStatus = new Dictionary<string, Color>
		{
			{ "open", Color.Green },
			{ "closed", Color.Red },
			{ "merged", Color.Purple }
		};

		public bool CanHandle(SocketUserMessage message)
		{
			return Regex.IsMatch(message.Content, RegexMatchPattern, RegexMatchCase ? RegexOptions.Compiled : RegexOptions.Compiled | RegexOptions.IgnoreCase);
		}

		public void Invoke(SocketUserMessage message)
		{
			var matches = Regex.Matches(message.Content, RegexMatchPattern, RegexMatchCase ? RegexOptions.Compiled : RegexOptions.Compiled | RegexOptions.IgnoreCase);
			foreach (Match match in matches)
			{
				var number = match.Value.Substring(IssueNumberStartPosition);

				var request = new RestRequest(ApiIssueRequestTemplate, Method.GET);
				request.AddUrlSegment("RepositoryOwner", RepositoryOwner);
				request.AddUrlSegment("RepositoryName", RepositoryName);
				request.AddUrlSegment("number", number);

				var response = _restClient.Execute<GitHubIssueResponse>(request);
				var issue = response.Data;
				if (issue?.HtmlUrl == null)
				{
					return;
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
					var pullRequest = new RestRequest(ApiPullRequestTemplate, Method.GET);
					pullRequest.AddUrlSegment("RepositoryOwner", RepositoryOwner);
					pullRequest.AddUrlSegment("RepositoryName", RepositoryName);
					pullRequest.AddUrlSegment("number", number);

					var pullResponse = _restClient.Execute<GitHubPullRequestResponse>(pullRequest);
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

				message.Channel.SendMessageAsync("", embed: embed.Build());
			}
		}

		private static string GetIssueIconUrl(bool isIssue, string status)
		{
			return $"{IssueIconBaseUrl}/{(isIssue ? "issue" : "pr")}-{status}.png";
		}
	}
}
