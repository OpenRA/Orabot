namespace Orabot.Objects.GitHub
{
	internal class GitHubPullRequestResponse
	{
		public bool merged { get; set; }

		public User merged_by { get; set; }

		public string mergeable_state { get; set; }
	}
}
