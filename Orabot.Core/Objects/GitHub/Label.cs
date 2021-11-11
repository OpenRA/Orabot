using RestSharp.Deserializers;

namespace Orabot.Core.Objects.GitHub
{
	internal class Label
	{
		[DeserializeAs(Name = "name")]
		public string Name { get; set; }
	}
}
