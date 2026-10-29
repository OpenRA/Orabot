using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orabot.Core.Objects;
using Orabot.Core.Services;

namespace Orabot.Core.Modules
{
	public class QuoteModule : ModuleBase<SocketCommandContext>
	{
		private readonly QuotingService _quotingService;
		private readonly string[] _trustedRoles;

		public QuoteModule(QuotingService quotingService, IConfiguration configuration)
		{
			_quotingService = quotingService;
			_trustedRoles = configuration["TrustedRoles"].Split(';').Select(x => x.Trim()).ToArray();
		}

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

			HandleQuote(Context.Guild, message);
		}

		[Command("quote")]
		[Summary("Quotes a single message specified by Message ID.")]
		[Remarks("Usage: `quote <messageId>`")]
		public async Task Quote(ulong messageId)
		{
			if (!_quotingService.TryGetMessage(Context.Guild, Context.Client.CurrentUser, messageId, out var message, Context.Channel as SocketTextChannel))
			{
				await ReplyAsync("No such message found in this server.");
				return;
			}

			HandleQuote(message);
		}

		[Command("quote")]
		[Summary("Quotes a group of messages specified by Message ID of the first and last messages. " +
				 "Only takes messages that belong to the author of the first one.")]
		[Remarks("Usage: `quote <firstMessageId> <lastMessageId>`")]
		public async Task Quote(ulong firstMessageId, ulong lastMessageId)
		{
			if (!_quotingService.TryGetMessage(Context.Guild, Context.Client.CurrentUser, firstMessageId, out var message, Context.Channel as SocketTextChannel))
			{
				await ReplyAsync("No such message found in this server.");
				return;
			}

			var messages = await _quotingService.GetMessageList(message.Channel as SocketTextChannel, firstMessageId, lastMessageId);
			if (!messages.Any())
			{
				await ReplyAsync("No such messages found in the target channel.");
				return;
			}

			HandleQuote(messages);
		}

		[Command("quote")]
		[Summary("Quotes a single message specified by Channel ID and Message ID.")]
		[Remarks("Usage: `quote <message_identifier>`")]
		public async Task Quote(DiscordMessageIdentifier messageIdentifier)
		{
			if (!_quotingService.TryGetChannel(Context.Guild, messageIdentifier.ChannelId, out var channel))
			{
				return;
			}

			var message = await channel.GetMessageAsync(messageIdentifier.MessageId);
			if (message == null)
			{
				await ReplyAsync("No such message found in the specified channel.");
				return;
			}

			HandleQuote(message);
		}

		[Command("quote")]
		[Summary("Quotes a single message specified by Channel ID and Message ID.")]
		[Remarks("Usage: `quote <firstMessageIdentifier> <lastMessageIdentifier>`")]
		public async Task Quote(DiscordMessageIdentifier firstMessageIdentifier, DiscordMessageIdentifier lastMessageIdentifier)
		{
			if (!_quotingService.TryGetChannel(Context.Guild, firstMessageIdentifier.ChannelId, out var channel))
			{
				return;
			}

			var messages = await _quotingService.GetMessageList(channel, firstMessageIdentifier.MessageId, lastMessageIdentifier.MessageId);
			if (!messages.Any())
			{
				await ReplyAsync("No such messages found in the specified channel.");
				return;
			}

			HandleQuote(messages);
		}

		[Command("quote")]
		[Summary("Quotes a single message specified by Message ID.")]
		[Remarks("Usage: `quote <#channel_name> <messageId>`")]
		public async Task Quote(string channelMention, ulong messageId)
		{
			if (!_quotingService.TryGetChannel(Context.Guild, channelMention, out var channel))
			{
				await ReplyAsync("Unknown channel specified.");
				return;
			}

			var message = await channel.GetMessageAsync(messageId);
			HandleQuote(message);
		}

		[Command("quote")]
		[Summary("Quotes a group of messages specified by Message ID of the first and last messages. " +
				 "Only takes messages that belong to the author of the first one.")]
		[Remarks("Usage: `quote <#channel_name> <firstMessageId> <lastMessageId>`")]
		public async Task Quote(string channelMention, ulong firstMessageId, ulong lastMessageId)
		{
			if (!_quotingService.TryGetChannel(Context.Guild, channelMention, out var channel))
			{
				await ReplyAsync("Unknown channel specified.");
				return;
			}

			var messages = await _quotingService.GetMessageList(channel, firstMessageId, lastMessageId);
			HandleQuote(messages);
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

			if (!_quotingService.TryGetGuild(Context.Client, guildId, out var guild) || !_quotingService.TryGetChannel(guild, channelId, out var channel))
			{
				return;
			}

			var message = await channel.GetMessageAsync(messageId);
			HandleQuote(message);
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

			if (!_quotingService.TryGetGuild(Context.Client, guildId, out var guild) || !_quotingService.TryGetChannel(guild, channelId, out var channel))
			{
				return;
			}

			var lastMessageIdentifier = lastMessageUri.AbsolutePath.Split('/').Last();
			if (!ulong.TryParse(lastMessageIdentifier, out var lastMessageId))
			{
				return;
			}

			var messages = await _quotingService.GetMessageList(channel, firstMessageId, lastMessageId);
			HandleQuote(messages);
		}

		#region Private methods

		private async void HandleQuote(SocketGuild guild, string message)
		{
			var embed = _quotingService.CreateEmbed(guild, message);
			TryDeleteQuoteCommand();
			await SendQuote(Context.User, embed);
		}

		private async void HandleQuote(IMessage message)
		{
			var embed = _quotingService.CreateEmbed(message);
			TryDeleteQuoteCommand();
			await SendQuote(Context.User, embed);
		}

		private async void HandleQuote(IList<IMessage> messages)
		{
			var embed = _quotingService.CreateEmbed(messages);
			TryDeleteQuoteCommand();
			await SendQuote(Context.User, embed);
		}

		private async Task SendQuote(IUser user, Embed embed)
		{
			await ReplyAsync($"{user.Username}:", false, embed);
		}

		private async void TryDeleteQuoteCommand()
		{
			try
			{
				await Context.Channel.DeleteMessageAsync(Context.Message, new RequestOptions
				{
					AuditLogReason = "Quote command!"
				});
			}
			catch (Exception) { }
		}

		#endregion
	}
}
