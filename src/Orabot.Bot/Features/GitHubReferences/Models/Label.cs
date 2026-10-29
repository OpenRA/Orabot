using System.Text.Json.Serialization;

namespace Orabot.Bot.Features.GitHubReferences.Models
{
	internal class Label
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }
	}
}
