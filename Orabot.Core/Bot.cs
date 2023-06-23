using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orabot.Core.Abstractions;
using Orabot.Core.Abstractions.EventHandlers;
using Orabot.Core.TypeReaders;

namespace Orabot.Core
{
	public class Bot : IDisposable
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly string _discordBotToken;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly CancellationToken _cancellationToken;

		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;

		private readonly ILogEventHandler _logEventHandler;
		private readonly IMessageEventHandler _messageEventHandler;
		private readonly IReactionEventHandler _reactionEventHandler;

		public Bot(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			_discordBotToken = _serviceProvider.GetRequiredService<IConfiguration>()["BotToken"];

			_client = _serviceProvider.GetService<DiscordSocketClient>();
			_commands = _serviceProvider.GetService<CommandService>();
			_logEventHandler = _serviceProvider.GetService<ILogEventHandler>();
			_messageEventHandler = _serviceProvider.GetService<IMessageEventHandler>();
			_reactionEventHandler = _serviceProvider.GetService<IReactionEventHandler>();

			_cancellationTokenSource = new CancellationTokenSource();
			_cancellationToken = _cancellationTokenSource.Token;
		}

		public async Task RunAsync()
		{
			AttachEventHandlers();
			await RegisterCommandModules();

			await _client.LoginAsync(TokenType.Bot, _discordBotToken);
			await _client.StartAsync();

			StartLongRunningServices();

			Console.ReadLine();

			await _client.LogoutAsync();
			await _client.StopAsync();

			Console.WriteLine("Done");
			Console.ReadLine();
		}

		public void Dispose()
		{
			_cancellationTokenSource.Cancel();
			((IDisposable)_commands)?.Dispose();
			_client?.Dispose();
		}

		#region Private methods

		private void AttachEventHandlers()
		{
			_client.Log += _logEventHandler.Log;
			_client.ReactionAdded += _reactionEventHandler.HandleReactionAddedAsync;
			_client.ReactionRemoved += _reactionEventHandler.HandleReactionRemovedAsync;
			_client.MessageReceived += _messageEventHandler.HandleMessageReceivedAsync;
		}

		private async Task RegisterCommandModules()
		{
			var typeReaders = _serviceProvider.GetServices<BaseTypeReader>();
			foreach (var typeReader in typeReaders)
				_commands.AddTypeReader(typeReader.SupportedType, typeReader);

			var modules = _serviceProvider.GetServices<ModuleBase<SocketCommandContext>>();
			foreach (var module in modules)
				await _commands.AddModuleAsync(module.GetType(), _serviceProvider);
		}

		private void StartLongRunningServices()
		{
			var services = _serviceProvider.GetServices<ILongRunningService>();
			foreach (var service in services)
				Task.Run(() => service.ExecuteAsync(_cancellationToken));
		}

		#endregion
	}
}
