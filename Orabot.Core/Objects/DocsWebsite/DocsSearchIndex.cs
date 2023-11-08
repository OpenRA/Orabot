using System.Text.Json.Serialization;

namespace Orabot.Core.Objects.DocsWebsite
{
	public class DocsSearchIndex
	{
		[JsonPropertyName("config")]
		public object Config { get; set; }

		[JsonPropertyName("docs")]
		public DocsEntry[] Docs { get; set; }
	}
}
