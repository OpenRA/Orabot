using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Orabot.Objects.OpenRaResourceCenter;
using RestSharp;

namespace Orabot.Transformers.LinkToEmbedTransformers
{
	internal class OpenRaResourceCenterMapLinkToEmbedTransformer
	{
		private const string BaseUrl = "https://resource.openra.net";
		private const string ApiMapInfoByUidTemplate = "map/hash/{uid}";
		private const string ApiMapInfoByNumberTemplate = "map/id/{number}";

		private readonly IRestClient _restClient;

		public OpenRaResourceCenterMapLinkToEmbedTransformer(IRestClient restClient)
		{
			_restClient = restClient;
			restClient.BaseUrl = new Uri(BaseUrl);
		}

		internal Embed CreateEmbed(string mapUid)
		{
			var request = new RestRequest(ApiMapInfoByUidTemplate, Method.GET);
			request.AddUrlSegment("uid", mapUid);

			var response = _restClient.Execute<List<MapInfo>>(request);
			var mapInfo = response.Data?.FirstOrDefault();
			if (mapInfo?.Title == null || mapInfo.Author == null)
			{
				return null;
			}

			return CreateEmbedInner(mapInfo);
		}

		internal Embed CreateEmbed(int number)
		{
			var request = new RestRequest(ApiMapInfoByNumberTemplate, Method.GET);
			request.AddUrlSegment("number", number);

			var response = _restClient.Execute<List<MapInfo>>(request);
			var mapInfo = response.Data?.FirstOrDefault();
			if (mapInfo?.Title == null || mapInfo.Author == null)
			{
				return null;
			}

			return CreateEmbedInner(mapInfo);
		}

		#region Private methods

		private Embed CreateEmbedInner(MapInfo mapInfo)
		{
			var bounds = mapInfo.Bounds.Split(',').Select(int.Parse).ToArray();
			var size = $"{bounds[2]}x{bounds[3]}";
			var color = GetColor($"mod_{mapInfo.GameMod}");
			var number = mapInfo.Id;

			var url = $"{BaseUrl}/maps/{number}";
			var description = mapInfo.Info.Length > 250 ? mapInfo.Info.Substring(0, 250) + "..." : mapInfo.Info;
			var authorUrl = Uri.EscapeUriString($"{BaseUrl}/maps/author/{mapInfo.Author}/");
			var minimapUrl = $"{BaseUrl}/maps/{number}/minimap";

			var embed = new EmbedBuilder
			{
				Title = $"{mapInfo.Title}\n({mapInfo.GameMod.ToUpper()}, {mapInfo.Players} players, {size})",
				ThumbnailUrl = minimapUrl,
				Url = url,
				Description = description,
				Author = new EmbedAuthorBuilder
				{
					Name = mapInfo.Author,
					Url = authorUrl
				},
				Footer = new EmbedFooterBuilder
				{
					Text = $"Published on {mapInfo.PostedOn}"
				},
				Color = color
			};

			return embed.Build();
		}

		private Color? GetColor(string modIdentifier)
		{
			var stylesheetLink = $"{BaseUrl}/static/style003.css";

			var request = new RestRequest(stylesheetLink, Method.GET);
			var response = _restClient.Execute(request);

			if (!response.Content.Contains(modIdentifier))
			{
				return null;
			}

			var hexColor = response.Content.Substring(response.Content.IndexOf(modIdentifier, StringComparison.Ordinal));
			hexColor = hexColor.Substring(hexColor.IndexOf('#'));
			hexColor = hexColor.Substring(1, hexColor.IndexOf(';') - 1);

			var r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
			var g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
			var b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

			return new Color(r, g, b);
		}

		#endregion
	}
}
