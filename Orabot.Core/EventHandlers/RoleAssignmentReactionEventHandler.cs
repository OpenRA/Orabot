﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orabot.Core.Abstractions.EventHandlers;

namespace Orabot.Core.EventHandlers
{

	internal class RoleAssignmentReactionEventHandler : IReactionEventHandler
	{
		private readonly ulong _roleAssignmentMessageId;
		private readonly IReadOnlyDictionary<string, string> _freelyAssignedRoles;

		public RoleAssignmentReactionEventHandler(IConfiguration configuration)
		{
			_roleAssignmentMessageId = ulong.Parse(configuration["RoleAssignmentMessageId"]);
			_freelyAssignedRoles = configuration["FreelyAssignedRolesByEmote"]
				.Split(';')
				.Select(x => x.Trim())
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Split(':'))
				.ToDictionary(x => x[0], y => y[1]);
		}

		public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> messageGetter, Cacheable<IMessageChannel, ulong> channelGetter, SocketReaction reaction)
		{
			var message = await messageGetter.GetOrDownloadAsync();
			var channel = await channelGetter.GetOrDownloadAsync();
			if (!CanHandle(channel, message, reaction, out var guildUser, out var targetRole))
			{
				return;
			}
			
			if (guildUser.Roles.All(x => x.Name != targetRole.Name))
			{
				await guildUser.AddRoleAsync(targetRole, new RequestOptions { AuditLogReason = "User asked for role." });
			}
		}

		public async Task HandleReactionRemovedAsync(Cacheable<IUserMessage, ulong> messageGetter, Cacheable<IMessageChannel, ulong> channelGetter, SocketReaction reaction)
		{
			var message = await messageGetter.GetOrDownloadAsync();
			var channel = await channelGetter.GetOrDownloadAsync();
			if (!CanHandle(channel, message, reaction, out var guildUser, out var targetRole))
			{
				return;
			}

			if (guildUser.Roles.Any(x => x.Name == targetRole.Name))
			{
				await guildUser.RemoveRoleAsync(targetRole, new RequestOptions { AuditLogReason = "User asked that role be removed." });
			}
		}

		private bool CanHandle(IMessageChannel channel, IMessage message, SocketReaction reaction, out SocketGuildUser guildUser, out SocketRole targetRole)
		{
			guildUser = null;
			targetRole = null;

			var user = reaction.User.Value;
			var guild = (channel as SocketGuildChannel)?.Guild;
			if (guild == null || !reaction.User.IsSpecified || reaction.User.Value == null)
			{
				return false;
			}

			if (message.Id != _roleAssignmentMessageId)
			{
				return false;
			}

			if (!_freelyAssignedRoles.TryGetValue(reaction.Emote.Name, out var targetRoleName))
			{
				return false;
			}

			var requestedRole = guild.Roles.FirstOrDefault(x => x.Name == targetRoleName);
			if (requestedRole == null)
			{
				return false;
			}

			if (!(user is SocketGuildUser socketGuildUser))
			{
				return false;
			}

			guildUser = socketGuildUser;
			targetRole = requestedRole;
			return true;
		}
	}
}
