using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orabot.Core.Abstractions;
using Orabot.Core.Integrations.ResourceCenter;
using Orabot.Core.Objects.OpenRaResourceCenter;
using Orabot.Core.Transformers.LinkToEmbedTransformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orabot.Core.WatcherServices
{
	internal class ResourceCenterMapWatcherService : ILongRunningService
	{
		private readonly string ServerName;
		private readonly string ChannelName;
		private readonly int ScanInterval;

		private readonly DiscordSocketClient _discordClient;
		private readonly IMapsApi _mapsApi;
		private readonly OpenRaResourceCenterMapLinkToEmbedTransformer _toEmbedTransformer;

		private int lastKnownMapId = -1;

		public ResourceCenterMapWatcherService(
			DiscordSocketClient discordClient,
			IConfiguration configuration,
			IMapsApi mapsApi,
			OpenRaResourceCenterMapLinkToEmbedTransformer toEmbedTransformer)
		{
			_discordClient = discordClient;
			_mapsApi = mapsApi;
			_toEmbedTransformer = toEmbedTransformer;

			var mapAnnouncer = configuration.GetRequiredSection("MapAnnouncer");
			ServerName = mapAnnouncer["ServerName"];
			ChannelName = mapAnnouncer["ChannelName"];
			ScanInterval = int.Parse(mapAnnouncer["ScanInterval"]);
		}

		public async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					Console.WriteLine($"Checking resource center for new maps... {DateTime.Now}");

					var response = await _mapsApi.GetMaps();
					var maps = response.Values;
					if (lastKnownMapId == -1)
						lastKnownMapId = maps.First().Id;
					else
						await HandleNewMaps(maps);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}

				await Task.Delay(ScanInterval);
			}
		}

		private async Task HandleNewMaps(IEnumerable<MapInfo> maps)
		{
			var guild = _discordClient.Guilds.FirstOrDefault(x => x.Name == ServerName);
			var channel = guild?.Channels?.FirstOrDefault(x => x.Name == ChannelName);

			foreach (var map in maps.Take(10).Reverse())
			{
				if (map.Id <= lastKnownMapId)
					continue;

				var embed = await _toEmbedTransformer.CreateEmbed(map);
				if (embed != null && channel is ISocketMessageChannel messageChannel)
					await messageChannel.SendMessageAsync($"**{map.Uploader}** uploaded a map:", embed: embed);

				lastKnownMapId = map.Id;

				// Pause for a second between maps.
				await Task.Delay(1000);
			}
		}
	}
}
