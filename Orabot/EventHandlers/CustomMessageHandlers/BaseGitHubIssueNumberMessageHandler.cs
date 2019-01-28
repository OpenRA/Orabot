using System.Collections.Generic;
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
		protected abstract int IssueNumberStartPosition { get; }

		protected abstract string RegexMatchPattern { get; }

		protected abstract bool RegexMatchCase { get; }

		protected abstract string RepositoryOwner { get; }

		protected abstract string RepositoryName { get; }

		public string HandlingCategory { get; } = nameof(BaseGitHubIssueNumberMessageHandler);

		public abstract int HandlingPriority { get; }

		private const string BaseApiUrl = "https://api.github.com";

		private const string ApiIssueRequestTemplate = "repos/{RepositoryOwner}/{RepositoryName}/issues/{number}";

		private const string ApiPullRequestTemplate = "repos/{RepositoryOwner}/{RepositoryName}/pulls/{number}";

		private readonly RestClient _restClient = new RestClient(BaseApiUrl);
		private readonly Dictionary<string, Color> ColorPerStatus = new Dictionary<string, Color>
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
				if (issue?.html_url == null)
				{
					return;
				}

				var isIssue = issue.pull_request == null;
				var type = isIssue ? "Issue" : "Pull request";
				var labels = string.Join(", ", issue.labels?.Select(x => x.name) ?? Enumerable.Empty<string>());
				var embedFields = new List<EmbedFieldBuilder>();
				if (!string.IsNullOrEmpty(labels))
				{
					embedFields.Add(new EmbedFieldBuilder
					{
						Name = "Labels:",
						Value = labels,
						IsInline = false
					});
				}

				var status = issue.state;

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
							Value = pull.mergeable_state,
							IsInline = true
						});

						status = pull.merged ? "merged" : issue.state;
						if (pull.merged)
						{
							embedFields.Add(new EmbedFieldBuilder
							{
								Name = "Merged by:",
								Value = pull.merged_by?.login,
								IsInline = true
							});
						}
					}
				}

				var embed = new EmbedBuilder
				{
					Title = issue.title,
					ThumbnailUrl = issue.user?.avatar_url,
					Url = issue.html_url,
					Description = issue.body.Length > 250 ? issue.body.Substring(0, 250) + "..." : issue.body,
					Author = new EmbedAuthorBuilder
					{
						Name = $"{type} #{number} by {issue.user?.login}  ({status})",
						IconUrl = issue.user?.avatar_url,
						Url = issue.html_url
					},
					Fields = embedFields,
					Footer = new EmbedFooterBuilder
					{
						Text = $"Created at {issue.created_at}"
					},
					Timestamp = issue.updated_at,
					Color = ColorPerStatus[status]
				};

				message.Channel.SendMessageAsync("", embed: embed.Build());
			}
		}
	}
}
