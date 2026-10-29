using System.Collections.Generic;

namespace Orabot.Bot.Features.Documentation.Models
{
	public class ParsedDocs
	{
		public IReadOnlyDictionary<string, DocsEntry> Traits { get; init; }

		public IReadOnlyDictionary<string, DocsEntry> Weapons { get; init; }

		public IReadOnlyDictionary<string, DocsEntry> SpriteSequences { get; init; }

		public IReadOnlyDictionary<string, DocsEntry> Lua { get; init; }
	}
}
