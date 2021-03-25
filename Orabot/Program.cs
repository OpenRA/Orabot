using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Orabot.EventHandlers;
using Orabot.EventHandlers.Abstraction;
using Orabot.EventHandlers.CustomMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.AttachmentMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.CommandMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.LinkParsingMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.ModTimersMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers;
using Orabot.EventHandlers.CustomMessageHandlers.SpecificTextMessageHandlers;
using Orabot.Modules;
using Orabot.Services;
using Orabot.Transformers.AttachmentToMessageTransformers;
using Orabot.Transformers.LinkToEmbedTransformers;
using Orabot.Transformers.Replays.ReplayDataToEmbedTransformers;
using Orabot.Transformers.Replays.ReplayToReplayDataTransformers;
using Orabot.TypeReaders;
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
				.AddSingleton<BaseTypeReader, UriTypeReader>()
				.AddSingleton<BaseTypeReader, DiscordMessageIdentifierTypeReader>()
				.AddSingleton<ILogEventHandler, LogEventHandler>()
				.AddSingleton<IMessageEventHandler, MessageEventHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaModSdkGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaWebGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OrabotGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaResourceCenterMapNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaResourceCenterMapLinkMessageHandler>()
				.AddSingleton<ICustomMessageHandler, LogFileAttachmentMessageHandler>()
				.AddSingleton<ICustomMessageHandler, ReplayFileAttachmentMessageHandler>()
				.AddSingleton<ICustomMessageHandler, BaseCommandMessageHandler>()
				.AddSingleton<ICustomMessageHandler, BaseModTimerMessageHandler>()
				.AddSingleton<ICustomMessageHandler, StackTraceMessageHandler>()
				.AddSingleton<ModuleBase<SocketCommandContext>, GeneralModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaGeneralModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaTraitsModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, OpenRaWeaponsModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, QuoteModule>()
				.AddSingleton<ModuleBase<SocketCommandContext>, RoleManagementModule>()
				.AddSingleton<QuotingService>()
				.AddSingleton<AttachmentLogFileToMessageTransformer>()
				.AddSingleton<OpenRaResourceCenterMapLinkToEmbedTransformer>()
				.AddSingleton<AttachmentReplayToUtilityMetadataTransformer>()
				.AddSingleton<UtilityReplayMetadataToEmbedTransformer>()
				.AddTransient<IRestClient, RestClient>()
				.AddTransient<YamlDotNet.Serialization.Deserializer>()
				.BuildServiceProvider();

			using (var bot = new Bot(serviceProvider))
			{
				bot.RunAsync().Wait();
			}
		}
	}
}
