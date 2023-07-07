﻿using Discord;
using System.Collections.Generic;

namespace Orabot.Core
{
	internal static class SlashCommands
	{
		private static ApplicationCommandOptionChoiceProperties[] documentationVersionChoices => new[]
		{
			new ApplicationCommandOptionChoiceProperties
			{
				Name = "release",
				Value = "release"
			},
			new ApplicationCommandOptionChoiceProperties
			{
				Name = "playtest",
				Value = "playtest"
			},
			new ApplicationCommandOptionChoiceProperties
			{
				Name = "development",
				Value = "development"
			}
		};

		public static IEnumerable<SlashCommandBuilder> CommandBuilders => new[]
		{
			new SlashCommandBuilder
			{
				Name = "lua",
				Description = "Provides a link to the OpenRA Lua API documentation page.",
				IsDMEnabled = true,
				IsNsfw = false
			}
				.AddOption("name", ApplicationCommandOptionType.String, "The name of the Lua table to link to.", isAutocomplete: true)
				.AddOption("version",
					ApplicationCommandOptionType.String,
					"Which documentation version to link to? Defaults to release.",
					choices: documentationVersionChoices),
			new SlashCommandBuilder
			{
				Name = "traits",
				Description = "Provides a link to the OpenRA Traits documentation page.",
				IsDMEnabled = true,
				IsNsfw = false
			}
				.AddOption("name", ApplicationCommandOptionType.String, "The name of the trait to link to.", isAutocomplete: true)
				.AddOption("version",
					ApplicationCommandOptionType.String,
					"Which documentation version to link to? Defaults to release.",
					choices: documentationVersionChoices),
			new SlashCommandBuilder
			{
				Name = "weapons",
				Description = "Provides a link to the OpenRA Weapons documentation page.",
				IsDMEnabled = true,
				IsNsfw = false
			}
				.AddOption("name", ApplicationCommandOptionType.String, "The name of the weapon (Warhead or Projectile) to link to.", isAutocomplete: true)
				.AddOption("version",
					ApplicationCommandOptionType.String,
					"Which documentation version to link to? Defaults to release.",
					choices: documentationVersionChoices),
			new SlashCommandBuilder
			{
				Name = "sprite-sequences",
				Description = "Provides a link to the OpenRA Sprite Sequences documentation page.",
				IsDMEnabled = true,
				IsNsfw = false
			}
				.AddOption("name", ApplicationCommandOptionType.String, "The name of the sprite sequence type to link to.", isAutocomplete: true)
				.AddOption("version",
					ApplicationCommandOptionType.String,
					"Which documentation version to link to? Defaults to release.",
					choices: documentationVersionChoices)
		};
	}
}
