using System.Threading.Tasks;
using Discord.Commands;
using Orabot.Core.Transformers.DocumentationToEmbedTransformers;

namespace Orabot.Core.Modules
{
	public class OpenRaLuaApiModule : ModuleBase<SocketCommandContext>
	{
		private readonly LuaTableToEmbedTransformer _luaTableToEmbedTransformer;

		public OpenRaLuaApiModule(LuaTableToEmbedTransformer luaTableToEmbedTransformer)
		{
			_luaTableToEmbedTransformer = luaTableToEmbedTransformer;
		}

		[Command("lua")]
		[Summary("Provides a link to the OpenRA Lua API documentation page. Can be used with an optional table name to link directly.")]
		public async Task LuaApi(string tableName = null)
		{
			var embed = await _luaTableToEmbedTransformer.CreateEmbed(tableName, "release");
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("lua-pt")]
		[Summary("Provides a link to the OpenRA playtest Lua API documentation page. Can be used with an optional table name to link directly.")]
		public async Task LuaApiPt(string tableName = null)
		{
			var embed = await _luaTableToEmbedTransformer.CreateEmbed(tableName, "playtest");
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("lua-dev")]
		[Summary("Provides a link to the OpenRA development Lua API documentation page. Can be used with an optional table name to link directly.")]
		public async Task LuaApiDev(string tableName = null)
		{
			var embed = await _luaTableToEmbedTransformer.CreateEmbed(tableName, "development");
			if (embed != null)
				await ReplyAsync("", false, embed);
		}
	}
}
