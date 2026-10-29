using System;
using System.Threading.Tasks;
using Discord;

namespace Orabot.Bot.Discord.Logging
{
	internal class LogEventHandler : ILogEventHandler
	{
		public Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
	}
}
