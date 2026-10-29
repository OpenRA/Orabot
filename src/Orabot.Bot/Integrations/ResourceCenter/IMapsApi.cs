using Orabot.Core.Objects.OpenRaResourceCenter;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orabot.Core.Integrations.ResourceCenter
{
	public interface IMapsApi
	{
		[Get("/api/maps")]
		Task<Dictionary<string, MapInfo>> GetMaps();
	}
}
