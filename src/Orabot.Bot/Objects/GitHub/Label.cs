using System.Text.Json.Serialization;

namespace Orabot.Core.Objects.GitHub
{
	internal class Label
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }
	}
}
