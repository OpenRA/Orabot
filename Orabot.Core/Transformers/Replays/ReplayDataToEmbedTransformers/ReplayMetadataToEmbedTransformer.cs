using System.Collections.Generic;
using System.Linq;
using Discord;
using Orabot.Core.Objects.OpenRaReplay;
using Orabot.Core.Transformers.LinkToEmbedTransformers;

namespace Orabot.Core.Transformers.Replays.ReplayDataToEmbedTransformers
{
	internal class ReplayMetadataToEmbedTransformer
	{
		private readonly OpenRaResourceCenterMapLinkToEmbedTransformer _mapToEmbedTransformer;

		public ReplayMetadataToEmbedTransformer(OpenRaResourceCenterMapLinkToEmbedTransformer mapToEmbedTransformer)
		{
			_mapToEmbedTransformer = mapToEmbedTransformer;
		}

		internal Embed CreateEmbed(ReplayMetadata replayMetadata, string replayLink = null)
		{
			var mapEmbed = _mapToEmbedTransformer.CreateEmbed(replayMetadata.MapUid);

			var fields = new List<EmbedFieldBuilder>
			{
				new EmbedFieldBuilder
				{
					IsInline = true,
					Name = "Mod:",
					Value = replayMetadata.Mod
				},
				new EmbedFieldBuilder
				{
					IsInline = true,
					Name = "Version:",
					Value = replayMetadata.Version
				},
				new EmbedFieldBuilder
				{
					IsInline = true,
					Name = "Map:",
					Value = mapEmbed == null ? $"{replayMetadata.MapTitle}" : $"[{replayMetadata.MapTitle}]({mapEmbed.Url})"
				},
				new EmbedFieldBuilder
				{
					IsInline = false,
					Name = "Duration:",
					Value = $"||{replayMetadata.EndTimeUtc - replayMetadata.StartTimeUtc}||"
				}
			};

			var playersByTeam = replayMetadata.Players.Values.GroupBy(x => x.Team).OrderBy(x => x.Key);
			var teamsCountStr = new List<string>();
			foreach (var kvp in playersByTeam)
			{
				if (kvp.Key == 0)
				{
					teamsCountStr.AddRange(kvp.Select(_ => "1"));

					fields.Add(new EmbedFieldBuilder
					{
						IsInline = true,
						Name = "No team:",
						Value = string.Join("\n", kvp.Select(x => x))
					});
				}
				else
				{
					teamsCountStr.Add(kvp.Count().ToString());
					fields.Add(new EmbedFieldBuilder
					{
						IsInline = true,
						Name = $"Team {kvp.Key}",
						Value = string.Join("\n", kvp.Select(x => x))
					});
				}
			}

			var winners = replayMetadata.Players.Values.Where(x => x.Outcome == "Won").ToArray();
			string winnerString;
			if (winners.Length == 0)
			{
				winnerString = "Winner is unknown.";
			}
			else if (winners.Length == 1)
			{
				winnerString = $"Winner is {winners[0].Name}";
			}
			else
			{
				winnerString = $"Winners are {string.Join(", ", winners.Select(x => x.Name))}";
			}

			var embed = new EmbedBuilder
			{
				Title = replayMetadata.FileName,
				ThumbnailUrl = mapEmbed?.Thumbnail?.Url,
				Url = replayLink,
				Description = $"This is a {string.Join("vs", teamsCountStr)} game. || {winnerString} ||",
				Footer = new EmbedFooterBuilder
				{
					Text = $"Played {replayMetadata.StartTimeUtc}"
				},
				Color = mapEmbed?.Color ?? Color.Default,
				Fields = fields
			};

			return embed.Build();
		}
	}
}
