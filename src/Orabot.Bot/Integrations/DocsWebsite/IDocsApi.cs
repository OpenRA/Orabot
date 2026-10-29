using Orabot.Core.Objects.DocsWebsite;
using Refit;
using System.Threading.Tasks;

namespace Orabot.Core.Integrations.DocsWebsite
{
	public interface IDocsApi
	{
		[Get("/{version}/search/search_index.json")]
		Task<DocsSearchIndex> GetSearchIndex(string version);
	}
}
