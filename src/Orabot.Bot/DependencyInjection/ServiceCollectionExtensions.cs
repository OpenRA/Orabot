using System;
using System.IO;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orabot.Bot.Discord;
using Orabot.Bot.Infrastructure.Configuration;
using Orabot.Bot.Discord.Commands;
using Orabot.Bot.Discord.Interactions;
using Orabot.Bot.Discord.Logging;
using Orabot.Bot.Discord.Messages;
using Orabot.Bot.Features.Documentation;
using Orabot.Bot.Features.Documentation.Autocomplete;
using Orabot.Bot.Features.Documentation.Commands;
using Orabot.Bot.Features.Documentation.Rendering;
using Orabot.Bot.Features.General;
using Orabot.Bot.Features.GitHubReferences.Messages;
using Orabot.Bot.Features.Logs;
using Orabot.Bot.Features.Maps;
using Orabot.Bot.Features.Maps.Messages;
using Orabot.Bot.Features.Maps.Rendering;
using Orabot.Bot.Features.Replays;
using Orabot.Bot.Features.Roles;
using Orabot.Bot.Infrastructure.BackgroundWork;
using Orabot.Bot.Infrastructure.Serialization;
using Refit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using System.Linq;

namespace Orabot.Bot.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddOrabotOptions(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddOptions<DiscordBotOptions>()
				.Configure(options =>
				{
					options.BotToken = configuration["BotToken"];
					options.MessageCacheSize = ReadInt<DiscordBotOptions>(configuration, "MessageCacheSize", 100);
				})
				.Validate(options => !string.IsNullOrWhiteSpace(options.BotToken), "BotToken must be configured.")
				.Validate(options => options.MessageCacheSize >= 0 && options.MessageCacheSize <= 10000,
					"MessageCacheSize must be between 0 and 10000.")
				.ValidateOnStart();
			services.AddOptions<DocumentationCacheOptions>()
				.Configure(options => options.ScanInterval = ReadInt<DocumentationCacheOptions>(configuration,
					"DocsSearchIndex:ScanInterval", 300000))
				.Validate(options => options.ScanInterval > 0, "DocsSearchIndex:ScanInterval must be positive milliseconds.")
				.ValidateOnStart();
			services.AddOptions<MapAnnouncerOptions>()
				.Configure(options =>
				{
					options.ServerName = configuration["MapAnnouncer:ServerName"];
					options.ChannelName = configuration["MapAnnouncer:ChannelName"];
					options.ScanInterval = ReadInt<MapAnnouncerOptions>(configuration, "MapAnnouncer:ScanInterval", 60000);
				})
				.Validate(options => !string.IsNullOrWhiteSpace(options.ServerName), "MapAnnouncer:ServerName must be configured.")
				.Validate(options => !string.IsNullOrWhiteSpace(options.ChannelName), "MapAnnouncer:ChannelName must be configured.")
				.Validate(options => options.ScanInterval > 0, "MapAnnouncer:ScanInterval must be positive milliseconds.")
				.ValidateOnStart();
			services.AddSingleton<IValidateOptions<LegacyFeatureOptions>, LegacyFeatureOptionsValidator>();
			services.AddOptions<LegacyFeatureOptions>().Bind(configuration).ValidateOnStart();
			return services;
		}

		public static IServiceCollection AddAppSettingsConfiguration(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton<IConfiguration>(provider => new ConfigurationBuilder()
						.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json")
						.AddUserSecrets("245384e8-58dc-45d7-b441-5c63468071e4")
						.Build());
		}

		public static IServiceCollection AddDefaultEventHandlerServices(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton<ILogEventHandler, LogEventHandler>()
				.AddSingleton<IMessageEventHandler, MessageEventHandler>()
				.AddSingleton<IReactionEventHandler, RoleAssignmentReactionEventHandler>()
				.AddSingleton<ISlashCommandEventHandler, SlashCommandEventHandler>()
				.AddSingleton<IAutocompleteEventHandler, AutocompleteEventHandler>();
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

		public static IServiceCollection AddDefaultSlashCommandHandlers(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton(provider => provider.GetServices<ILongRunningService>().Single(x => x is DocsCachingService) as DocsCachingService)
				.AddSingleton<ISlashCommandHandler, OpenRaInformationCommandHandler>()
				.AddSingleton<ISlashCommandHandler, LuaDocumentationCommandHandler>()
				.AddSingleton<ISlashCommandHandler, TraitDocumentationCommandHandler>()
				.AddSingleton<ISlashCommandHandler, WeaponDocumentationCommandHandler>()
				.AddSingleton<ISlashCommandHandler, SpriteSequenceDocumentationCommandHandler>();
		}

		public static IServiceCollection AddDefaultAutocompletionHandlers(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton<IAutocompleteHandler, LuaTableNamesAutocompleteHandler>()
				.AddSingleton<IAutocompleteHandler, SpriteSequenceNamesAutocompleteHandler>()
				.AddSingleton<IAutocompleteHandler, TraitNamesAutocompleteHandler>()
				.AddSingleton<IAutocompleteHandler, WeaponNamesAutocompleteHandler>();
		}

		public static IServiceCollection AddDefaultTransformers(this IServiceCollection serviceCollection)
		{
			return serviceCollection
				.AddSingleton<AttachmentLogFileToMessageTransformer>()
				.AddSingleton<OpenRaResourceCenterMapLinkToEmbedTransformer>()
				.AddSingleton<AttachmentReplayMetadataTransformer>()
				.AddSingleton<TraitToEmbedTransformer>()
				.AddSingleton<WeaponToEmbedTransformer>()
				.AddSingleton<LuaTableToEmbedTransformer>()
				.AddSingleton<SpriteSequenceToEmbedTransformer>()
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
				.AddSingleton<ILongRunningService, DocsCachingService>()
				.AddSingleton<ILongRunningService, ResourceCenterMapWatcherService>();
		}

		public static IServiceCollection AddDocumentationWebsiteIntegration(this IServiceCollection serviceCollection)
		{
			var serializerOptions = new JsonSerializerOptions();
			var settings = new RefitSettings
			{
				ContentSerializer = new SystemTextJsonContentSerializer(serializerOptions)
			};

			serviceCollection
				.AddRefitClient<IDocsApi>(settings)
				.ConfigureHttpClient(c => c.BaseAddress = new Uri("https://docs.openra.net/en/"));

			return serviceCollection;
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
		private static int ReadInt<TOptions>(IConfiguration configuration, string key, int defaultValue)
		{
			var value = configuration[key];
			if (value == null)
				return defaultValue;
			if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
				return result;
			throw new OptionsValidationException(Options.DefaultName, typeof(TOptions),
				new[] { key + " must be a 32-bit integer." });
		}

	}
}
