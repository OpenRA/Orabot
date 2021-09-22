namespace Orabot.Core.Objects
{
	public class DiscordMessageIdentifier
	{
		public ulong ChannelId { get; set; }

		public ulong MessageId { get; set; }

		public DiscordMessageIdentifier(ulong channelId, ulong messageId)
		{
			ChannelId = channelId;
			MessageId = messageId;
		}

		public static bool TryParse(string input, out DiscordMessageIdentifier value)
		{
			if (TryParseString(input, out var channelId, out var messageId))
			{
				value = new DiscordMessageIdentifier(channelId, messageId);
				return true;
			}

			value = null;
			return false;
		}

		private static bool TryParseString(string input, out ulong channelId, out ulong messageId)
		{
			channelId = 0;
			messageId = 0;

			if (string.IsNullOrWhiteSpace(input))
			{
				return false;
			}

			var parts = input.Split('-');
			if (parts.Length != 2)
			{
				return false;
			}

			return ulong.TryParse(parts[0], out channelId) && ulong.TryParse(parts[1], out messageId);
		}
	}
}
