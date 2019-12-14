using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Orabot.Modules
{
	public class QuoteModule : ModuleBase<SocketCommandContext>
	{
		private readonly string[] _trustedRoles = ConfigurationManager.AppSettings["TrustedRoles"].Split(';').Select(x => x.Trim()).ToArray();


		[Command("quote")]
		[Summary("Quotes an arbitrary message with optional author, source channel and timestamp. Limited usage for some roles only.")]
		[Remarks("Usage: `quote [#channel_name]\n[author][time]\n<text to quote>`")]
		public async Task Quote([Remainder]string message)
		{
			var userRoles = (Context.User as SocketGuildUser)?.Roles;
			if (userRoles == null || !userRoles.Select(x => x.Name).Intersect(_trustedRoles).Any())
			{
				await ReplyAsync("No rights!");
				return;
			}

			await Context.Channel.DeleteMessageAsync(Context.Message);

			var embed = CreateEmbed(message);
			await SendQuote(Context.User, embed);
		}

		[Command("quote")]
		[Summary("Quotes a single message from the current channel, specified by Message ID.")]
		[Remarks("Usage: `quote <messageId>`")]
		public async Task Quote(ulong messageId)
		{
			var message = await Context.Channel.GetMessageAsync(messageId);
			if (message == null)
			{
				await ReplyAsync("No such message found in the current channel.");
				return;
			}

			await Context.Channel.DeleteMessageAsync(Context.Message, RequestOptions.Default);

			var embed = CreateEmbed(message);
			await SendQuote(Context.User, embed);
		}

		[Command("quote")]
		[Summary("Quotes a group of messages specified by Message ID of the first and last messages. " +
		         "Only takes messages that belong to the author of the first one.")]
		[Remarks("Usage: `quote <firstMessageId> <lastMessageId>`")]
		public async Task Quote(ulong firstMessageId, ulong lastMessageId)
		{
			var messages = await GetMessageList(Context.Channel as SocketTextChannel, firstMessageId, lastMessageId);
			if (!messages.Any())
			{
				await ReplyAsync("No such messages found in the current channel.");
				return;
			}

			await Context.Channel.DeleteMessageAsync(Context.Message, RequestOptions.Default);

			var embed = CreateEmbed(messages);
			await SendQuote(Context.User, embed);
		}

		[Command("quote")]
		[Summary("Quotes a single message specified by Message ID.")]
		[Remarks("Usage: `quote <#channel_name> <messageId>`")]
		public async Task Quote(string channelMention, ulong messageId)
		{
			if (!TryGetChannel(channelMention, out var channel))
			{
				await ReplyAsync("Unknown channel specified.");
				return;
			}

			await Context.Channel.DeleteMessageAsync(Context.Message, RequestOptions.Default);

			var message = await channel.GetMessageAsync(messageId);
			var embed = CreateEmbed(message);
			await SendQuote(Context.User, embed);
		}

		[Command("quote")]
		[Summary("Quotes a group of messages specified by Message ID of the first and last messages. " +
				 "Only takes messages that belong to the author of the first one.")]
		[Remarks("Usage: `quote <#channel_name> <firstMessageId> <lastMessageId>`")]
		public async Task Quote(string channelMention, ulong firstMessageId, ulong lastMessageId)
		{
			if (!TryGetChannel(channelMention, out var channel))
			{
				await ReplyAsync("Unknown channel specified.");
				return;
			}

			await Context.Channel.DeleteMessageAsync(Context.Message, RequestOptions.Default);

			var messages = await GetMessageList(channel, firstMessageId, lastMessageId);
			var embed = CreateEmbed(messages);
			await SendQuote(Context.User, embed);
		}

		[Command("quote")]
		[Summary("Quotes a single message specified by Message URI.")]
		[Remarks("Usage: `quote <messageUri>`")]
		public async Task Quote(Uri messageUri)
		{
			var parts = messageUri.AbsolutePath.Split('/').Reverse().ToArray();

			if (!ulong.TryParse(parts[0], out var messageId) ||
				!ulong.TryParse(parts[1], out var channelId) ||
				!ulong.TryParse(parts[2], out var guildId))
			{
				return;
			}

			if (!TryGetGuild(guildId, out var guild) || !TryGetChannel(guild, channelId, out var channel))
			{
				return;
			}

			await Context.Channel.DeleteMessageAsync(Context.Message, RequestOptions.Default);

			var message = await channel.GetMessageAsync(messageId);
			var embed = CreateEmbed(message);
			await SendQuote(Context.User, embed);
		}

		[Command("quote")]
		[Summary("Quotes a group of messages specified by Message URI of the first and last messages. " +
				 "Only takes messages that belong to the author of the first one.")]
		[Remarks("Usage: `quote <firstMessageUri> <lastMessageUri>`")]
		public async Task Quote(Uri firstMessageUri, Uri lastMessageUri)
		{
			var parts = firstMessageUri.AbsolutePath.Split('/').Reverse().ToArray();

			if (!ulong.TryParse(parts[0], out var firstMessageId) ||
				!ulong.TryParse(parts[1], out var channelId) ||
				!ulong.TryParse(parts[2], out var guildId))
			{
				return;
			}

			if (!TryGetGuild(guildId, out var guild) || !TryGetChannel(guild, channelId, out var channel))
			{
				return;
			}

			var lastMessageIdentifier = lastMessageUri.AbsolutePath.Split('/').Last();
			if (!ulong.TryParse(lastMessageIdentifier, out var lastMessageId))
			{
				return;
			}

			await Context.Channel.DeleteMessageAsync(Context.Message, RequestOptions.Default);

			var messages = await GetMessageList(channel, firstMessageId, lastMessageId);
			var embed = CreateEmbed(messages);
			await SendQuote(Context.User, embed);
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

		private bool TryGetChannel(string channelMention, out SocketTextChannel channel)
		{
			foreach (var socketGuildChannel in Context.Guild.Channels)
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

		private static bool TryGetChannel(SocketGuild guild, ulong channelId, out SocketTextChannel channel)
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

		private bool TryGetGuild(ulong guildId, out SocketGuild guild)
		{
			foreach (var clientGuild in Context.Client.Guilds)
			{
				if (clientGuild.Id == guildId)
				{
					guild = clientGuild;
					return true;
				}
			}

			guild = null;
			return false;
		}

		private async Task<List<IMessage>> GetMessageList(SocketTextChannel channel, ulong firstMessageId, ulong lastMessageId)
		{
			var firstMessage = await channel.GetMessageAsync(firstMessageId);

			var downloadedMessages = channel.GetMessagesAsync(firstMessageId, Direction.After, 25);
			var firstPage = (await downloadedMessages.First()).ToArray();

			var messages = new List<IMessage>
			{
				firstMessage
			};

			for (var i = firstPage.Length - 1; i >= 0; i--)
			{
				if (firstPage[i].Author == firstMessage.Author)
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
				footerText = $"  -  {timestamp}";
			}

			if (referredChannel != null)
			{
				footerText += $", in #{referredChannel.Name}";
			}

			return new EmbedFooterBuilder
			{
				Text = footerText
			};
		}

		private Embed CreateEmbed(string message)
		{
			var lines = message.Split('\n').ToList();

			if (TryGetChannel(lines[0].Trim(), out var referredChannel))
			{
				lines.RemoveAt(0);
			}

			if (TryGetQuoteAuthorAndTimestamp(lines[0].Trim(), Context.Channel, out var author, out var authorName, out var timestamp))
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

		private static Embed CreateEmbed(IMessage message)
		{
			var author = message.Author;
			var authorName = (author as SocketGuildUser)?.Nickname ?? author.Username;
			var referredChannel = message.Channel;
			var timestamp = message.Timestamp.ToString();

			var embed = new EmbedBuilder
			{
				Color = Color.Blue,
				Author = BuildAuthorEmbed(author, authorName),
				Description = message.Content,
				Footer = BuildFooterEmbed(referredChannel, timestamp)
			};

			return embed.Build();
		}

		private static Embed CreateEmbed(IList<IMessage> messages)
		{
			var message = messages.First();
			var author = message.Author;
			var authorName = (author as SocketGuildUser)?.Nickname ?? author.Username;
			var referredChannel = message.Channel;
			var timestamp = message.Timestamp.ToString();

			var embed = new EmbedBuilder
			{
				Color = Color.Blue,
				Author = BuildAuthorEmbed(author, authorName),
				Description = string.Join("\n", messages.Select(x => x.Content)),
				Footer = BuildFooterEmbed(referredChannel, timestamp)
			};

			return embed.Build();
		}

		private async Task SendQuote(IUser user, Embed embed)
		{
			await ReplyAsync($"{user.Username}:", false, embed);
		}

		#endregion
	}
}
