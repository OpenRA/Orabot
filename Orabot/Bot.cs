using System;
using System.Configuration;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Orabot.EventHandlers;
using Orabot.EventHandlers.Abstraction;
using Orabot.Modules;

namespace Orabot
{
	public class Bot : IDisposable
	{
		private static readonly string DiscordBotToken = ConfigurationManager.AppSettings["BotToken"];

		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly IServiceProvider _serviceProvider;

		private readonly ILogEventHandler _logEventHandler;
		private readonly IMessageEventHandler _messageEventHandler;

		public Bot()
		{
			_client = new DiscordSocketClient();
			_commands = new CommandService();
			_serviceProvider = new ServiceCollection()
				.AddSingleton(_client)
				.AddSingleton(_commands)
				.BuildServiceProvider();

			_logEventHandler = new LogEventHandler();
			_messageEventHandler = new MessageEventHandler(_serviceProvider);

			AttachEventHandlers();
			RegisterCommandModules();
		}

		public async Task RunAsync()
		{
			await _client.LoginAsync(TokenType.Bot, DiscordBotToken);
			await _client.StartAsync();

			Console.ReadLine();

			await _client.LogoutAsync();
			await _client.StopAsync();

			Console.WriteLine("Done");
			Console.ReadLine();
		}

		public void Dispose()
		{
			((IDisposable) _commands)?.Dispose();
			_client?.Dispose();
		}

		#region Private methods

		private void AttachEventHandlers()
		{
			_client.Log += _logEventHandler.Log;
			_client.MessageReceived += _messageEventHandler.HandleMessageReceivedAsync;
		}

		private void RegisterCommandModules()
		{
			_commands.AddModuleAsync<GeneralModule>(_serviceProvider);
		}

		#endregion
	}
}
