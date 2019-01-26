using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Orabot.EventHandlers;

namespace Orabot
{
	public class Bot : IDisposable
	{
		private static readonly string discordBotToken = "";

		private readonly CommandService _commands;
		private readonly DiscordSocketClient _client;

		public Bot()
		{
			_client = new DiscordSocketClient();
			_commands = new CommandService();

			AttachEventHandlers();
		}

		public async Task RunAsync()
		{
			await _client.LoginAsync(TokenType.Bot, discordBotToken);
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
			_client.Log += LogHandler.Log;
			_client.MessageReceived += MessageReceivedHandler.HandleMessageReceivedAsync;
		}

		#endregion
	}
}
