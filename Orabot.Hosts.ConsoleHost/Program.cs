using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Orabot.Core;
using Orabot.Core.DependencyInjection;
using Orabot.Core.Modules;
using Orabot.Core.Services;
using Orabot.Core.TypeReaders;
using RestSharp;
using RestSharp.Serializers.Json;
using System.Text.Json;

namespace Orabot.Hosts.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
			var serviceProvider = new ServiceCollection()
				.AddAppSettingsConfiguration()
				.AddSingleton(_ => new DiscordSocketClient(new DiscordSocketConfig
				{
					GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
				}))
				.AddSingleton<CommandService>()
				.AddSingleton<BaseTypeReader, UriTypeReader>()
				.AddSingleton<BaseTypeReader, DiscordMessageIdentifierTypeReader>()
				.AddDefaultEventHandlerServices()
				.AddDefaultCustomMessageHandlers()
				.AddSingleton<ModuleBase<SocketCommandContext>, GeneralModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaGeneralModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaLuaApiModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaTraitsModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaWeaponsModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, QuoteModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, RoleManagementModule>()
				.AddSingleton<QuotingService>()
				.AddDefaultTransformers()
				.AddSingleton<IRestClient>(_ =>
				{
					var serializationOptions = new JsonSerializerOptions();
					serializationOptions.Converters.Add(new CustomDateTimeConverter());

					return new RestClient(
					   new RestClientOptions(),
					   configureSerialization: s => s.UseSystemTextJson(serializationOptions));
				})
				.AddYamlDeserializer()
				.BuildServiceProvider();

			using var bot = new Bot(serviceProvider);
			bot.RunAsync().Wait();
		}
    }
}
