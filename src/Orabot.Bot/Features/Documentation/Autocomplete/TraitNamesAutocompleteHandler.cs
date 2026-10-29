using Discord.WebSocket;
using Orabot.Bot.Discord.Interactions;
using Orabot.Bot.Features.Documentation;
using Orabot.Bot.Features.Documentation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orabot.Bot.Features.Documentation.Autocomplete
{
	public class TraitNamesAutocompleteHandler : BaseDocumentationAutocompleteHandler, IAutocompleteHandler
	{
		public TraitNamesAutocompleteHandler(DocsCachingService docsCachingService)
			: base(docsCachingService) { }

		public bool CanHandle(SocketAutocompleteInteraction interaction)
			=> interaction.Data.CommandName == "traits";

		public async Task InvokeAsync(SocketAutocompleteInteraction interaction)
			=> await InvokeInner(interaction);

		protected override IReadOnlyDictionary<string, DocsEntry> GetDocsEntries(ParsedDocs docsCache)
			=> docsCache.Traits;
	}
}
