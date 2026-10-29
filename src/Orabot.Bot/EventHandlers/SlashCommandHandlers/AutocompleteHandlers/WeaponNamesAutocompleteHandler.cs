using Discord.WebSocket;
using Orabot.Core.LongRunningServices;
using Orabot.Core.Objects.DocsWebsite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orabot.Core.EventHandlers.SlashCommandHandlers.AutocompleteHandlers
{
	public class WeaponNamesAutocompleteHandler : BaseDocumentationAutocompleteHandler, Abstractions.EventHandlers.IAutocompleteHandler
	{
		public WeaponNamesAutocompleteHandler(DocsCachingService docsCachingService)
			: base(docsCachingService) { }

		public bool CanHandle(SocketAutocompleteInteraction interaction)
			=> interaction.Data.CommandName == "weapons";

		public async Task InvokeAsync(SocketAutocompleteInteraction interaction)
			=> await InvokeInner(interaction);

		protected override IReadOnlyDictionary<string, DocsEntry> GetDocsEntries(ParsedDocs docsCache)
			=> docsCache.Weapons;
	}
}
