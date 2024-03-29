﻿using Discord;
using Discord.WebSocket;
using Orabot.Core.Abstractions.EventHandlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orabot.Core.EventHandlers.SlashCommandHandlers
{
	internal class OpenRaInformationCommandHandler : ISlashCommandHandler
	{
		private readonly IReadOnlyDictionary<string, Func<SocketSlashCommand, Task>> _commands = new Dictionary<string, Func<SocketSlashCommand, Task>>()
		{
			{ "supportdir", async command => await SupportDir(command) },
			{ "academy", async command => await Academy(command) },
			{ "book", async command => await Book(command) },
			{ "sdk", async command => await Sdk(command) },
			{ "utility", async command => await Utility(command) },
			{ "orabot", async command => await Orabot(command) },
		};

		public bool CanHandle(SocketSlashCommand command) => _commands.ContainsKey(command.CommandName);

		public async Task InvokeAsync(SocketSlashCommand command) => await _commands[command.CommandName](command);

		private static async Task SupportDir(SocketSlashCommand command)
		{
			await command.RespondAsync("```" +
							 "The support directory is where the game keeps the saved settings, maps, replays, logs and mod assets.  The default location is as follows, but one can choose to move it to an arbitrary location by passing an Engine.SupportDir argument to the Game.exe\n\n" +
							 "Windows:      %APPDATA%\\OpenRA\\\n" +
							 "macOS:        ~/Library/Application Support/OpenRA/\n" +
							 "GNU/Linux:    $XDG_CONFIG_HOME/openra/openra\n\n" +
							 "Older releases (before playtest-20190825) used different locations, which newer versions may continue to use in some circumstances:\n\n" +
							 "Windows:      %USERPROFILE%\\Documents\\OpenRA\\\n" +
							 "GNU/Linux:    ~/.openra/\n" +
							 "```");
		}

		private static async Task Academy(SocketSlashCommand command)
		{
			const string discordServerInvite = "https://discord.gg/C2CadJT";
			const string permalink = "http://academy.openra.net/";

			var embedBuilder = new EmbedBuilder
			{
				Description = "The OpenRA Academy is a separate Discord server aimed at helping players get better at the game.\n" +
							  $"Here is a [Discord invite]({discordServerInvite}), or you could use the permalink {permalink}."
			};

			await command.RespondAsync(embed: embedBuilder.Build());
		}

		private static async Task Book(SocketSlashCommand command)
		{
			const string bookUrl = "https://www.openra.net/book/";
			const string bookRepoUrl = "https://github.com/OpenRA/book";

			var embedBuilder = new EmbedBuilder
			{
				Description = $"[The OpenRA book]({bookUrl}) aims to teach new players how to play and existing players how to get better.\n" +
							  "It also covers creating new games on the OpenRA engine and how the engine works under-the-hood.\n" +
							  $"You can find the official repository for the book [on GitHub]({bookRepoUrl})."
			};

			await command.RespondAsync(embed: embedBuilder.Build());
		}

		private static async Task Sdk(SocketSlashCommand command)
		{
			const string sdkRepoUrl = "https://github.com/OpenRA/OpenRAModSDK";

			var embedBuilder = new EmbedBuilder
			{
				Description = $"[The OpenRA ModSDK]({sdkRepoUrl}) helps you build your own games using the OpenRA engine."
			};

			await command.RespondAsync(embed: embedBuilder.Build());
		}

		private static async Task Utility(SocketSlashCommand command)
		{
			const string utilityWikiUrl = "https://github.com/OpenRA/OpenRA/wiki/Utility";

			var embedBuilder = new EmbedBuilder
			{
				Description = $"The OpenRA [Utility]({utilityWikiUrl}) is an executable that ships with the engine.\n" +
							  $"It is run through the command line to perform a wide range of operations like manipulating game assets " +
							  $"and checking mod YAML files for errors."
			};

			await command.RespondAsync(embed: embedBuilder.Build());
		}

		private static async Task Orabot(SocketSlashCommand command)
		{
			const string orabotRepoUrl = "https://github.com/OpenRA/Orabot";

			var embedBuilder = new EmbedBuilder
			{
				Description = $"[Orabot]({orabotRepoUrl}) is a modern successor to the famous OpenRA companion - the IRC orabot."
			};

			await command.RespondAsync(embed: embedBuilder.Build());
		}
	}
}
