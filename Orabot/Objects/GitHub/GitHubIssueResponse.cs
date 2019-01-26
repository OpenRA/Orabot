using System;
using System.Collections.Generic;

namespace Orabot.Objects.GitHub
{
	internal class GitHubIssueResponse
	{
		public string html_url { get; set; }

		public string state { get; set; }

		public string title { get; set; }

		public string body { get; set; }

		public object pull_request { get; set; }

		public DateTime created_at { get; set; }

		public DateTimeOffset? updated_at { get; set; }

		public User user { get; set; }

		public List<Label> labels { get; set; }
	}
}
