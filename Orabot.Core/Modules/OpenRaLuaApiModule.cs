using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Orabot.Core.Modules
{
	public class OpenRaLuaApiModule : ModuleBase<SocketCommandContext>
	{
		private readonly IRestClient _restClient;
		private readonly string _openRaIconUrl;
		private readonly string _luaReleasePageUrl;
		private readonly string _luaPlaytestPageUrl;
		private readonly string _luaDevelopmentPageUrl;

		public OpenRaLuaApiModule(IConfiguration configuration, IRestClient restClient)
		{
			_restClient = restClient;

			_openRaIconUrl = configuration["OpenRaFaviconUrl"];

			var traitsPages = configuration.GetRequiredSection("Lua");
			_luaReleasePageUrl = traitsPages["ReleasePageUrl"];
			_luaPlaytestPageUrl = traitsPages["PlaytestPageUrl"];
			_luaDevelopmentPageUrl = traitsPages["DevelopmentPageUrl"];
		}

		[Command("lua")]
		[Summary("Provides a link to the OpenRA Lua API documentation page. Can be used with an optional table name to link directly.")]
		public async Task LuaApi(string tableName = null)
		{
			var embed = await BuildLuaApiPageEmbed(_luaReleasePageUrl, tableName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("lua-pt")]
		[Summary("Provides a link to the OpenRA playtest Lua API documentation page. Can be used with an optional table name to link directly.")]
		public async Task LuaApiPt(string tableName = null)
		{
			var embed = await BuildLuaApiPageEmbed(_luaPlaytestPageUrl, tableName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		[Command("lua-dev")]
		[Summary("Provides a link to the OpenRA development Lua API documentation page. Can be used with an optional table name to link directly.")]
		public async Task LuaApiDev(string tableName = null)
		{
			var embed = await BuildLuaApiPageEmbed(_luaDevelopmentPageUrl, tableName);
			if (embed != null)
				await ReplyAsync("", false, embed);
		}

		#region Private methods

		private async Task<bool> CheckTableExists(string pageUrl, string tableName)
		{
			var request = new RestRequest(pageUrl, Method.Get);
			var response = await _restClient.ExecuteAsync(request);
			return response.Content.Contains($"<a href=\"#{tableName.ToLower()}\"");
		}

		private async Task<Embed> BuildLuaApiPageEmbed(string pageUrl, string tableName)
		{
			if (string.IsNullOrWhiteSpace(pageUrl))
				return null;

			var hasName = !string.IsNullOrWhiteSpace(tableName);
			if (hasName)
				hasName = await CheckTableExists(pageUrl, tableName);

			var targetUrl = pageUrl + (hasName ? $"#{tableName.ToLower()}" : string.Empty);
			var embedBuilder = new EmbedBuilder
			{
				Author = new EmbedAuthorBuilder
				{
					Name = "OpenRA Lua API page" + (hasName ? $", table {tableName}" : string.Empty),
					Url = targetUrl,
					IconUrl = _openRaIconUrl
				},
				Title = targetUrl,
				Url = targetUrl,
				Description = "This documentation is aimed at scripted map creators."
			};

			return embedBuilder.Build();
		}

		#endregion
	}
}
