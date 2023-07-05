using Discord.WebSocket;
using Orabot.Core.Abstractions.EventHandlers;
using Orabot.Core.Transformers.DocumentationToEmbedTransformers;
using System.Linq;
using System.Threading.Tasks;

namespace Orabot.Core.EventHandlers.SlashCommandHandlers
{
	internal class LuaDocumentationCommandHandler : ISlashCommandHandler
	{
		private readonly LuaTableToEmbedTransformer _luaTableToEmbedTransformer;

		public LuaDocumentationCommandHandler(LuaTableToEmbedTransformer luaTableToEmbedTransformer)
		{
			_luaTableToEmbedTransformer = luaTableToEmbedTransformer;
		}

		public bool CanHandle(SocketSlashCommand command) => command.CommandName == "lua";

		public async Task InvokeAsync(SocketSlashCommand command)
		{
			var tableName = (string)command.Data.Options.FirstOrDefault(x => x.Name == "table-name")?.Value;
			var version = (string)command.Data.Options.FirstOrDefault(x => x.Name == "version")?.Value ?? "release";

			var embed = await _luaTableToEmbedTransformer.CreateEmbed(tableName, version);
			if (embed != null)
				await command.RespondAsync(embed: embed);
		}
	}
}
