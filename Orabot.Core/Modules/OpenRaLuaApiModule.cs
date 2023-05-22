using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Orabot.Core.Modules
{
	public class OpenRaLuaApiModule : ModuleBase<SocketCommandContext>
	{
		private const string LuaPageUrl = "https://docs.openra.net/en/release/lua/";
		private const string LuaPlaytestPageUrl = "https://docs.openra.net/en/playtest/lua/";

		private readonly IRestClient _restClient;
		private readonly string _openRaIconUrl;

		public OpenRaLuaApiModule(IConfiguration configuration, IRestClient restClient)
		{
			_restClient = restClient;
			_restClient.BaseUrl = new Uri(LuaPageUrl);
			_openRaIconUrl = configuration["OpenRaFaviconUrl"];
		}

		[Command("lua")]
		[Summary("Provides a link to the OpenRA Lua API documentation page. Can be used with an optional table name to link directly.")]
		public async Task LuaApi(string tableName = null)
		{
			var embed = BuildLuaApiPageEmbed(LuaPageUrl, tableName);
			await ReplyAsync("", false, embed);
		}

		[Command("lua-pt")]
		[Summary("Provides a link to the OpenRA playtest Lua API documentation page. Can be used with an optional table name to link directly.")]
		public async Task LuaApiPt(string tableName = null)
		{
			var embed = BuildLuaApiPageEmbed(LuaPlaytestPageUrl, tableName);
			await ReplyAsync("", false, embed);
		}

		#region Private methods

		private bool CheckTableExists(string tableName)
		{
			var request = new RestRequest(Method.Get);
			var response = _restClient.Execute(request);
			return response.Content.Contains($"<a href=\"#{tableName.ToLower()}\"");
		}

		private Embed BuildLuaApiPageEmbed(string pageUrl, string tableName)
		{
			var hasName = !string.IsNullOrWhiteSpace(tableName);
			if (hasName)
			{
				hasName = CheckTableExists(tableName);
			}

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
