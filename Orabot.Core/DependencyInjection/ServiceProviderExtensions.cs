using System;
using System.IO;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orabot.Core.Abstractions;
using Orabot.Core.Abstractions.EventHandlers;
using Orabot.Core.EventHandlers;
using Orabot.Core.EventHandlers.CustomMessageHandlers.AttachmentMessageHandlers;
using Orabot.Core.EventHandlers.CustomMessageHandlers.CommandMessageHandlers;
using Orabot.Core.EventHandlers.CustomMessageHandlers.LinkParsingMessageHandlers;
using Orabot.Core.EventHandlers.CustomMessageHandlers.ModTimersMessageHandlers;
using Orabot.Core.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers;
using Orabot.Core.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers.GitHubIssueNumberMessageHandlers;
using Orabot.Core.EventHandlers.CustomMessageHandlers.SpecificTextMessageHandlers;
using Orabot.Core.Integrations.ResourceCenter;
using Orabot.Core.Transformers.AttachmentToMessageTransformers;
using Orabot.Core.Transformers.DocumentationToEmbedTransformers;
using Orabot.Core.Transformers.LinkToEmbedTransformers;
using Orabot.Core.Transformers.Replays.ReplayDataToEmbedTransformers;
using Orabot.Core.Transformers.Replays.ReplayToReplayDataTransformers;
using Orabot.Core.WatcherServices;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using Refit;
using System.Text.Json;

namespace Orabot.Core.DependencyInjection
{
	public static class ServiceProviderExtensions
	{
		public static IServiceCollection AddAppSettingsConfiguration(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton<IConfiguration>(provider => new ConfigurationBuilder()
						.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json")
						.Build());
		}

		public static IServiceCollection AddDefaultEventHandlerServices(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton<ILogEventHandler, LogEventHandler>()
				.AddSingleton<IMessageEventHandler, MessageEventHandler>()
				.AddSingleton<IReactionEventHandler, RoleAssignmentReactionEventHandler>();
		}

		public static IServiceCollection AddDefaultCustomMessageHandlers(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton<ICustomMessageHandler, OpenRaGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaModSdkGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaRa2GitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaResourceCenterGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaWebGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OrabotGitHubIssueNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaResourceCenterMapNumberMessageHandler>()
				.AddSingleton<ICustomMessageHandler, OpenRaResourceCenterMapLinkMessageHandler>()
				.AddSingleton<ICustomMessageHandler, LogFileAttachmentMessageHandler>()
				.AddSingleton<ICustomMessageHandler, ReplayFileAttachmentMessageHandler>()
				.AddSingleton<ICustomMessageHandler, BaseCommandMessageHandler>()
				.AddSingleton<ICustomMessageHandler, BaseModTimerMessageHandler>()
				.AddSingleton<ICustomMessageHandler, StackTraceMessageHandler>();
		}

		public static IServiceCollection AddDefaultTransformers(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton<AttachmentLogFileToMessageTransformer>()
				.AddSingleton<OpenRaResourceCenterMapLinkToEmbedTransformer>()
				.AddSingleton<AttachmentReplayMetadataTransformer>()
				.AddSingleton<TraitToEmbedTransformer>()
				.AddSingleton<ReplayMetadataToEmbedTransformer>();
		}

		public static IServiceCollection AddYamlDeserializer(this IServiceCollection serviceCollection)
		{
			return serviceCollection.AddSingleton(deserializer =>
				new DeserializerBuilder()
					.IgnoreUnmatchedProperties()
					.WithTypeConverter(new DateTimeConverter(DateTimeKind.Utc, CultureInfo.InvariantCulture, false, "yyyy-MM-dd HH-mm-ss"))
					.Build()
			);
		}

		public static IServiceCollection AddLongRunningServices(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton<ILongRunningService, ResourceCenterMapWatcherService>();
		}

		public static IServiceCollection AddResourceCenterIntegration(this IServiceCollection serviceCollection)
		{
			var serializerOptions = new JsonSerializerOptions();
			serializerOptions.Converters.Add(new CustomDateTimeConverter());

			var settings = new RefitSettings
			{
				ContentSerializer = new SystemTextJsonContentSerializer(serializerOptions)
			};

			serviceCollection
				.AddRefitClient<IMapsApi>(settings)
				.ConfigureHttpClient(c => c.BaseAddress = new Uri("https://resource.openra.net"));

			return serviceCollection;
		}
	}
}
