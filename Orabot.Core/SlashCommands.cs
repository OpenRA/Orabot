using Discord;
using System.Collections.Generic;

namespace Orabot.Core
{
	internal static class SlashCommands
	{
		public static IEnumerable<SlashCommandBuilder> CommandBuilders => new[]
		{
			new SlashCommandBuilder
			{
				Name = "lua",
				Description = "Provides a link to the OpenRA Lua API documentation page.",
				IsDMEnabled = true,
				IsNsfw = false
			},
			new SlashCommandBuilder
			{
				Name = "traits",
				Description = "Provides a link to the OpenRA Traits documentation page.",
				IsDMEnabled = true,
				IsNsfw = false
			},
			new SlashCommandBuilder
			{
				Name = "weapons",
				Description = "Provides a link to the OpenRA Weapons documentation page.",
				IsDMEnabled = true,
				IsNsfw = false
			}
		};
	}
}
