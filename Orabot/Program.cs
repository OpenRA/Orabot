using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Orabot.EventHandlers;
using Orabot.EventHandlers.Abstraction;
using Orabot.EventHandlers.CustomMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.CommandMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.GitHubIssueNumberMessageHandlers;
using Orabot.Modules;
using RestSharp;

namespace Orabot
{
	internal class Program
	{
		private static void Main()
		{
			var serviceProvider = new ServiceCollection()
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton<CommandService>()
				.AddSingleton<ILogEventHandler, LogEventHandler>()
				.AddSingleton<IMessageEventHandler, MessageEventHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaWebGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, BaseCommandMessageHandler>()
				.AddSingleton<ModuleBase<SocketCommandContext>, GeneralModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaTraitsModule>()
				.AddTransient<IRestClient, RestClient>()
				.BuildServiceProvider();

			using (var bot = new Bot(serviceProvider))
			{
				bot.RunAsync().Wait();
			}
		}
	}
}
