using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Orabot.Modules
{
	public class RoleManagementModule : ModuleBase<SocketCommandContext>
	{
		private readonly string[] _freelyAssignedRoles = ConfigurationManager.AppSettings["FreelyAssignedRoles"].Split(';').Select(x => x.Trim()).ToArray();

		[Command("role")]
		[Summary("Adds or removes a specified role to the user.")]
		public async Task Role([Remainder]string roleName = "")
		{
			if (string.IsNullOrWhiteSpace(roleName))
			{
				await ReplyAsync("Command usage: `]role <roleName>`, where `<roleName>` is one of the following: " +
				                 $"{string.Join(',', _freelyAssignedRoles.Select(x => $"`{x}`"))}");
				return;
			}

			var targetRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == roleName);
			if (targetRole == null)
			{
				await ReplyAsync($":warning: No role with name `{roleName}` exists.");
				return;
			}

			if (!_freelyAssignedRoles.Contains(roleName))
			{
				await ReplyAsync($":warning: You don't have permission to self-assign role `{roleName}`.");
				return;
			}

			if (!(Context.User is SocketGuildUser guildUser))
			{
				await ReplyAsync(":x: Something went terribly wrong.");
				return;
			}

			if (guildUser.Roles.Any(x => x.Name == roleName))
			{
				await guildUser.RemoveRoleAsync(targetRole, new RequestOptions { AuditLogReason = "User asked that role be removed." });
				await ReplyAsync($":white_check_mark: Removed the `{roleName}` role from you.");
			}
			else
			{
				await guildUser.AddRoleAsync(targetRole, new RequestOptions { AuditLogReason = "User asked for role." });
				await ReplyAsync($":white_check_mark: Added the `{roleName}` role to you. Run the command again to remove it.");
			}
		}
	}
}
