using System.Text.Json.Serialization;

namespace Orabot.Bot.Features.Documentation.Models
{
	public class DocsEntry
	{
		[JsonPropertyName("location")]
		public string Location { get; set; }

		[JsonPropertyName("text")]
		public string Text { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }
	}
}
