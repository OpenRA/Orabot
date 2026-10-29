using Discord.WebSocket;
using Orabot.Bot.Features.Documentation;
using Orabot.Bot.Features.Documentation.Models;
using Orabot.Core.Abstractions.EventHandlers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orabot.Bot.Features.Documentation.Autocomplete
{
	public class SpriteSequenceNamesAutocompleteHandler : BaseDocumentationAutocompleteHandler, IAutocompleteHandler
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
