using System.Text.Json.Serialization;

namespace Orabot.Core.Objects.DocsWebsite
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
