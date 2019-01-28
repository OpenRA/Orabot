using RestSharp.Deserializers;

namespace Orabot.Objects.GitHub
{
	internal class GitHubPullRequestResponse
	{
		[DeserializeAs(Name = "merged")]
		public bool IsMerged { get; set; }

		[DeserializeAs(Name = "merged_by")]
		public User MergedBy { get; set; }

		[DeserializeAs(Name = "mergeable_state")]
		public string MergeableState { get; set; }
	}
}
