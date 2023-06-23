using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Orabot.Core.Objects.GitHub
{
	internal class GitHubIssueResponse
	{
		[JsonPropertyName("html_url")]
		public string HtmlUrl { get; set; }

		[JsonPropertyName("state")]
		public string State { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("body")]
		public string Body { get; set; }

		[JsonPropertyName("pull_request")]
		public object PullRequest { get; set; }

		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonPropertyName("updated_at")]
		public DateTime? UpdatedAt { get; set; }

		[JsonPropertyName("user")]
		public User User { get; set; }

		[JsonPropertyName("labels")]
		public List<Label> Labels { get; set; }
	}
}
