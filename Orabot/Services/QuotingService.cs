using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Orabot.Services
{
	public class QuotingService
	{
		public bool TryGetGuild(IDiscordClient discordClient, ulong guildId, out SocketGuild guild)
		{
			guild = discordClient.GetGuildAsync(guildId).Result as SocketGuild;
			return guild != null;
		}

		public bool TryGetChannel(SocketGuild guild, ulong channelId, out SocketTextChannel channel)
		{
			foreach (var socketGuildChannel in guild.Channels)
			{
				if (socketGuildChannel is SocketTextChannel tmpChannel && tmpChannel.Id == channelId)
				{
					channel = tmpChannel;
					return true;
				}
			}

			channel = null;
			return false;
		}

		public bool TryGetChannel(SocketGuild guild, string channelMention, out SocketTextChannel channel)
		{
			foreach (var socketGuildChannel in guild.Channels)
			{
				if (socketGuildChannel is SocketTextChannel tmpChannel && tmpChannel.Mention == channelMention)
				{
					channel = tmpChannel;
					return true;
				}
			}

			channel = null;
			return false;
		}

		public bool TryGetMessage(SocketGuild guild, IUser currentUser, ulong messageId, out IMessage message, SocketTextChannel preferredChannel = null)
		{
			var channels = new List<SocketTextChannel>(guild.TextChannels);
			if (preferredChannel != null)
			{
				channels.Insert(0, preferredChannel);
			}

			foreach (var channel in channels)
			{
				if (!channel.Users.Any(x => x.Username == currentUser.Username && x.DiscriminatorValue == currentUser.DiscriminatorValue))
				{
					continue;
				}

				message = channel.GetMessageAsync(messageId).Result;
				if (message != null)
				{
					return true;
				}
			}

			message = null;
			return false;
		}

		public async Task<List<IMessage>> GetMessageList(SocketTextChannel channel, ulong firstMessageId, ulong lastMessageId)
		{
			var firstMessage = await channel.GetMessageAsync(firstMessageId);
			if (firstMessage == null)
			{
				return new List<IMessage>();
			}

			var downloadedMessages = channel.GetMessagesAsync(firstMessageId, Direction.After, 25);
			var firstPage = (await downloadedMessages.First()).ToArray();

			var messages = new List<IMessage>
			{
				firstMessage
			};

			for (var i = firstPage.Length - 1; i >= 0; i--)
			{
				if (firstPage[i].Author.Id == firstMessage.Author.Id)
				{
					messages.Add(firstPage[i]);
				}

				if (firstPage[i].Id == lastMessageId)
				{
					break;
				}
			}

			return messages;
		}
		
		public Embed CreateEmbed(SocketGuild guild, string message)
		{
			var lines = message.Split('\n').ToList();

			if (TryGetChannel(guild, lines[0].Trim(), out var referredChannel))
			{
				lines.RemoveAt(0);
			}

			if (TryGetQuoteAuthorAndTimestamp(lines[0].Trim(), referredChannel, out var author, out var authorName, out var timestamp))
			{
				lines.RemoveAt(0);
			}

			var embed = new EmbedBuilder
			{
				Color = Color.Blue,
				Author = BuildAuthorEmbed(author, authorName),
				Description = string.Join("\n", lines),
				Footer = BuildFooterEmbed(referredChannel, timestamp)
			};

			return embed.Build();
		}

		public Embed CreateEmbed(IMessage message)
		{
			var author = message.Author;
			var authorName = (author as SocketGuildUser)?.Nickname ?? author.Username;
			var referredChannel = message.Channel;
			var timestamp = message.Timestamp.ToString("s").Replace('T', ' ') + " UTC";
			var descriptionText = $"{message.Content}\n\n[Original message]({message.GetJumpUrl()})";

			var embed = new EmbedBuilder
			{
				Color = Color.Blue,
				Author = BuildAuthorEmbed(author, authorName),
				Description = descriptionText,
				Footer = BuildFooterEmbed(referredChannel, timestamp)
			};

			return embed.Build();
		}

		public Embed CreateEmbed(IList<IMessage> messages)
		{
			var message = messages.First();
			var author = message.Author;
			var authorName = (author as SocketGuildUser)?.Nickname ?? author.Username;
			var referredChannel = message.Channel;
			var timestamp = message.Timestamp.ToString("s").Replace('T', ' ') + " UTC";
			var descriptionText = $"{string.Join("\n", messages.Select(x => x.Content))}\n\n[Original message]({message.GetJumpUrl()})";

			var embed = new EmbedBuilder
			{
				Color = Color.Blue,
				Author = BuildAuthorEmbed(author, authorName),
				Description = descriptionText,
				Footer = BuildFooterEmbed(referredChannel, timestamp)
			};

			return embed.Build();
		}

		#region Private methods

		private static IEnumerable<IUser> GetChannelUsers(IChannel channel)
		{
			var usersAdapter = channel.GetUsersAsync();
			var users = usersAdapter.ToList().Result.SelectMany(x => x);

			return users;
		}

		private static Dictionary<string, List<IUser>> GetChannelUsersDictionary(IChannel channel)
		{
			var users = GetChannelUsers(channel).ToList();
			var usersByUsername = users.GroupBy(x => x.Username);
			var usersByNickname = users.Cast<SocketGuildUser>().Where(x => !string.IsNullOrWhiteSpace(x.Nickname)).GroupBy(x => x.Nickname);
			var usersByNameTmp = usersByNickname.Union(usersByUsername).GroupBy(x => x.Key);
			var usersByName = usersByNameTmp.ToDictionary(x => x.Key, y => y.SelectMany(z => z).ToList());

			return usersByName;
		}

		private static bool TryGetQuoteAuthorAndTimestamp(string messageLine, IChannel channel, out IUser author, out string authorName, out string timestamp)
		{
			var usersByName = GetChannelUsersDictionary(channel);

			foreach (var userList in usersByName)
			{
				if (messageLine.StartsWith(userList.Key))
				{
					author = userList.Value.Count == 1 ? userList.Value.First() : null;
					authorName = userList.Key;

					var trim = messageLine.Substring(userList.Key.Length);
					timestamp = trim.Trim();

					return true;
				}
			}

			author = null;
			authorName = null;
			timestamp = null;
			return false;
		}

		private static EmbedAuthorBuilder BuildAuthorEmbed(IUser author, string authorNameToUse)
		{
			if (author == null)
			{
				return null;
			}

			return new EmbedAuthorBuilder
			{
				Name = authorNameToUse,
				IconUrl = author.GetAvatarUrl()
			};
		}

		private static EmbedFooterBuilder BuildFooterEmbed(IChannel referredChannel, string timestamp)
		{
			var footerText = string.Empty;

			if (!string.IsNullOrEmpty(timestamp))
			{
				footerText = timestamp;
			}

			if (referredChannel != null)
			{
				if (!string.IsNullOrEmpty(timestamp))
				{
					footerText += ", ";
				}

				footerText += $"in #{referredChannel.Name}";
			}

			return new EmbedFooterBuilder
			{
				Text = footerText
			};
		}

		#endregion
	}
}
