using System.Text.Json.Serialization;

namespace Orabot.Bot.Features.GitHubReferences.Models
{
	internal class User
	{
		[JsonPropertyName("login")]
		public string LoginName { get; set; }

		[JsonPropertyName("avatar_url")]
		public string AvatarUrl { get; set; }
	}
}
