﻿using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Orabot.EventHandlers;
using Orabot.EventHandlers.Abstraction;
using Orabot.EventHandlers.CustomMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.CommandMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.LinkParsingMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.ModTimersMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers;
using Orabot.Modules;
using Orabot.Transformers.LinkToEmbedTransformers;
using RestSharp;
using SteakBot.Core.TypeReaders;

namespace Orabot
{
	internal class Program
	{
		private static void Main()
		{
			var serviceProvider = new ServiceCollection()
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton<CommandService>()
				.AddSingleton<BaseTypeReader, UriTypeReader>()
				.AddSingleton<ILogEventHandler, LogEventHandler>()
				.AddSingleton<IMessageEventHandler, MessageEventHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaModSdkGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaWebGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OrabotGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaResourceCenterMapNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaResourceCenterMapLinkMessageHandler>()
				.AddSingleton<ICustomMessageHandler, BaseCommandMessageHandler>()
				.AddSingleton<ICustomMessageHandler, BaseModTimerMessageHandler>()
				.AddSingleton<ModuleBase<SocketCommandContext>, GeneralModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaGeneralModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaTraitsModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaWeaponsModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, QuoteModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, RoleManagementModule>()
				.AddSingleton<OpenRaResourceCenterMapLinkToEmbedTransformer>()
				.AddTransient<IRestClient, RestClient>()
				.BuildServiceProvider();

			using (var bot = new Bot(serviceProvider))
			{
				bot.RunAsync().Wait();
			}
		}
	}
}
