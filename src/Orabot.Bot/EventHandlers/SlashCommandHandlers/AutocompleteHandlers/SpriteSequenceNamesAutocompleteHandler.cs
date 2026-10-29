using Discord.WebSocket;
using Orabot.Core.LongRunningServices;
using Orabot.Core.Objects.DocsWebsite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orabot.Core.EventHandlers.SlashCommandHandlers.AutocompleteHandlers
{
	public class SpriteSequenceNamesAutocompleteHandler : BaseDocumentationAutocompleteHandler, Abstractions.EventHandlers.IAutocompleteHandler
	{
		public SpriteSequenceNamesAutocompleteHandler(DocsCachingService docsCachingService)
			: base(docsCachingService) { }

		public bool CanHandle(SocketAutocompleteInteraction interaction)
			=> interaction.Data.CommandName == "sprite-sequences";

		public async Task InvokeAsync(SocketAutocompleteInteraction interaction)
			=> await InvokeInner(interaction);

		protected override IReadOnlyDictionary<string, DocsEntry> GetDocsEntries(ParsedDocs docsCache)
			=> docsCache.SpriteSequences;
	}
}
