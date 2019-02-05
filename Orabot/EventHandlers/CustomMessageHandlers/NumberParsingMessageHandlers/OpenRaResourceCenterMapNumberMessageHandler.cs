using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using RestSharp;

namespace Orabot.EventHandlers.CustomMessageHandlers.NumberParsingMessageHandlers
{
	internal class OpenRaResourceCenterMapNumberMessageHandler : BaseNumberParsingMessageHandler
	{
		private const string BaseUrl = "https://resource.openra.net";

		protected override string[] RegexMatchPatternKeywords { get; } = { "map" };

		private readonly IRestClient _restClient;

		public OpenRaResourceCenterMapNumberMessageHandler(IRestClient restClient)
		{
			_restClient = restClient;
			restClient.BaseUrl = new Uri(BaseUrl);
		}

		public override void Invoke(SocketUserMessage message)
		{
			var web = new HtmlWeb();

			foreach (var number in GetMatchedNumbers(message.Content))
			{
				var doc = web.Load($"{BaseUrl}/maps/{number}");

				var title = doc.DocumentNode.SelectNodes("//div[contains(@class, \"title\")]").LastOrDefault()?.InnerText;
				if (string.IsNullOrWhiteSpace(title))
				{
					continue;
				}

				var author = doc.DocumentNode.SelectNodes("//div[contains(@class, \"author\")]").FirstOrDefault()?.InnerText;
				if (string.IsNullOrWhiteSpace(author))
				{
					continue;
				}

				author = author.Replace("by ", string.Empty);

				var imageUrlNode = doc.DocumentNode.SelectNodes("//img[contains(@alt, \"minimap\")]").SingleOrDefault(x => x.Attributes["src"].Value == $"/maps/{number}/minimap");
				var imageUrl = imageUrlNode?.Attributes["src"]?.Value;
				if (string.IsNullOrWhiteSpace(imageUrl))
				{
					continue;
				}

				imageUrl = $"{BaseUrl}{imageUrl}";

				var targetModNode = doc.DocumentNode.Descendants().SingleOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "span" && n.GetAttributeValue("class", string.Empty).StartsWith("mod_"));
				var targetModIdentifier = targetModNode?.GetAttributeValue("class", null);
				if (string.IsNullOrWhiteSpace(targetModIdentifier))
				{
					continue;
				}

				var targetMod = targetModNode.InnerText;

				var players = GetPlayerCount(doc);
				var size = GetMapSize(doc);

				var embed = new EmbedBuilder
				{
					Title = $"{title}  ({targetMod}, {players} players, {size})",
					ThumbnailUrl = imageUrl,
					Url = $"{BaseUrl}/maps/{number}",
					Description = GetDescription(doc),
					Author = new EmbedAuthorBuilder
					{
						Name = author,
						Url = $"{BaseUrl}/maps/author/{author}/"
					},
					Footer = new EmbedFooterBuilder
					{
						Text = $"Published at {GetPublishedDate(doc)}"
					},
					Color = GetColor(doc, targetModIdentifier)
				};

				message.Channel.SendMessageAsync("", embed: embed.Build());
			}
		}

		private Color? GetColor(HtmlDocument document, string modIdentifier)
		{
			var stylesheetLinkNode = document.DocumentNode.SelectSingleNode("/html[1]/head[1]/link[1]");
			var stylesheetLink = stylesheetLinkNode?.Attributes["href"]?.Value;
			if (string.IsNullOrWhiteSpace(stylesheetLink))
			{
				return null;
			}

			var request = new RestRequest(stylesheetLink, Method.GET);
			var response = _restClient.Execute(request);

			if (!response.Content.Contains(modIdentifier))
			{
				return null;
			}

			var hexColor = response.Content.Substring(response.Content.IndexOf(modIdentifier));
			hexColor = hexColor.Substring(hexColor.IndexOf('#'));
			hexColor = hexColor.Substring(1, hexColor.IndexOf(';') - 1);
			
			var r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
			var g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
			var b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

			return new Color(r, g, b);
		}

		private string GetDescription(HtmlDocument document)
		{
			var descriptionNode = document.DocumentNode.Descendants().SingleOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "div" && n.GetAttributeValue("class", string.Empty).StartsWith("map_description"));
			if (descriptionNode == null)
			{
				return null;
			}

			var description = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[3]/div[2]/div[1]/div[1]/div[6]/div[1]/p[1]/p[1]").InnerText;
			return description.Length > 250 ? description.Substring(0, 250) + "..." : description;
		}

		private string GetPlayerCount(HtmlDocument document)
		{
			var playersNode = document.DocumentNode.Descendants()
				.SingleOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "div"
				                                                         && n.GetAttributeValue("class", string.Empty) == "map_data_label"
				                                                         && n.InnerText == "Players:");
			return playersNode.NextSibling.InnerText;
		}

		private string GetMapSize(HtmlDocument document)
		{
			var sizeNode = document.DocumentNode.Descendants()
				.SingleOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "div"
				                                                         && n.GetAttributeValue("class", string.Empty) == "map_data_label"
				                                                         && n.InnerText == "Size:");
			return sizeNode.NextSibling.InnerText;
		}

		private string GetPublishedDate(HtmlDocument document)
		{
			var publishedNode = document.DocumentNode.Descendants()
				.SingleOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "div"
				                                                         && n.GetAttributeValue("class", string.Empty) == "map_data_label"
				                                                         && n.InnerText == "Published:");
			return publishedNode.NextSibling.InnerText;
		}
	}
}
