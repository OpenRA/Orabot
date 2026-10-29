using System.Text.Json.Serialization;

namespace Orabot.Core.Objects.GitHub
{
	internal class GitHubPullRequestResponse
	{
		[JsonPropertyName("merged")]
		public bool IsMerged { get; set; }

		[JsonPropertyName("merged_by")]
		public User MergedBy { get; set; }

		[JsonPropertyName("mergeable_state")]
		public string MergeableState { get; set; }
	}
}
