using Discord.Commands;

namespace Orabot.Core.Extensions
{
	public static class CommandInfoExtensions
	{
		public static string CustomToString(this CommandInfo commandInfo)
		{
			var res = $"<{commandInfo.Name}> - {commandInfo.Summary}";
			if (!string.IsNullOrWhiteSpace(commandInfo.Remarks))
			{
				res += $"\n    {commandInfo.Remarks}";
			}

			return res;
		}
	}
}
