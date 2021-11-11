using System;
using System.Collections.Generic;
using RestSharp.Deserializers;

namespace Orabot.Core.Objects.GitHub
{
	internal class GitHubIssueResponse
	{
		[DeserializeAs(Name = "html_url")]
		public string HtmlUrl { get; set; }

		[DeserializeAs(Name = "state")]
		public string State { get; set; }

		[DeserializeAs(Name = "title")]
		public string Title { get; set; }

		[DeserializeAs(Name = "body")]
		public string Body { get; set; }

		[DeserializeAs(Name = "pull_request")]
		public object PullRequest { get; set; }

		[DeserializeAs(Name = "created_at")]
		public DateTime CreatedAt { get; set; }

		[DeserializeAs(Name = "updated_at")]
		public DateTimeOffset? UpdatedAt { get; set; }

		[DeserializeAs(Name = "user")]
		public User User { get; set; }

		[DeserializeAs(Name = "labels")]
		public List<Label> Labels { get; set; }
	}
}
