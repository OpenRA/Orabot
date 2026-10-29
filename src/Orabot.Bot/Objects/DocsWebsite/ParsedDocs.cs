using System.Collections.Generic;

namespace Orabot.Core.Objects.DocsWebsite
{
	public class ParsedDocs
	{
		public IReadOnlyDictionary<string, DocsEntry> Traits { get; init; }

		public IReadOnlyDictionary<string, DocsEntry> Weapons { get; init; }

		public IReadOnlyDictionary<string, DocsEntry> SpriteSequences { get; init; }

		public IReadOnlyDictionary<string, DocsEntry> Lua { get; init; }
	}
}
