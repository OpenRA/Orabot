using System;
using System.Threading.Tasks;
using Discord;
using Orabot.EventHandlers.Abstraction;

namespace Orabot.EventHandlers
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
