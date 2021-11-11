using System;
using System.Threading.Tasks;
using Discord;
using Orabot.Core.Abstractions.EventHandlers;

namespace Orabot.Core.EventHandlers
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
