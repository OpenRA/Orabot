using System;
using System.Threading.Tasks;
using Discord;

namespace Orabot.EventHandlers
{
	internal static class LogHandler
	{
		public static Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
	}
}
