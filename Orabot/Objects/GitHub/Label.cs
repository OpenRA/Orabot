using RestSharp.Deserializers;

namespace Orabot.Objects.GitHub
{
	internal class Label
	{
		[DeserializeAs(Name = "name")]
		public string Name { get; set; }
	}
}
