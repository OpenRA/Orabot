using Orabot.Bot.Features.Documentation.Models;
using Refit;
using System.Threading.Tasks;

namespace Orabot.Bot.Features.Documentation
{
	public interface IDocsApi
	{
		[Get("/{version}/search/search_index.json")]
		Task<DocsSearchIndex> GetSearchIndex(string version);
	}
}
