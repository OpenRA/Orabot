using System;
using System.Linq;
using System.Web;
using Discord;
using HtmlAgilityPack;
using RestSharp;

namespace Orabot.Transformers.LinkToEmbedTransformers
{
	internal class OpenRaResourceCenterMapLinkToEmbedTransformer
	{
		private const string BaseUrl = "https://resource.openra.net";

		private readonly IRestClient _restClient;

		public OpenRaResourceCenterMapLinkToEmbedTransformer(IRestClient restClient)
		{
			_restClient = restClient;
			restClient.BaseUrl = new Uri(BaseUrl);
		}

		internal Embed CreateEmbed(string number = null)
		{
			var web = new HtmlWeb();
			var doc = web.Load($"{BaseUrl}/maps/{number}");

			if (!TryGetMapTitle(doc, out var title)
				|| !TryGetAuthor(doc, out var author)
			    || !TryGetMapPreviewUrl(doc, number, out var imageUrl)
			    || !TryGetTargetMod(doc, out var targetModIdentifier, out var targetModName))
			{
				return null;
			}

			var players = GetPlayerCount(doc);
			var size = GetMapSize(doc);
			var description = GetDescription(doc);
			var publishDate = GetPublishedDate(doc);
			var color = GetColor(doc, targetModIdentifier);

			var url = $"{BaseUrl}/maps/{number}";
			var authorUrl = Uri.EscapeUriString($"{BaseUrl}/maps/author/{author}/");

			var embed = new EmbedBuilder
			{
				Title = $"{title}  ({targetModName}, {players} players, {size})",
				ThumbnailUrl = imageUrl,
				Url = url,
				Description = description,
				Author = new EmbedAuthorBuilder
				{
					Name = author,
					Url = authorUrl
				},
				Footer = new EmbedFooterBuilder
				{
					Text = $"Published on {publishDate}"
				},
				Color = color
			};

			return embed.Build();
		}

		#region Private methods

		private bool TryGetMapTitle(HtmlDocument document, out string title)
		{
			title = document.DocumentNode.SelectNodes("//div[contains(@class, \"title\")]").LastOrDefault()?.InnerText;
			return !string.IsNullOrWhiteSpace(title);
		}

		private bool TryGetAuthor(HtmlDocument document, out string author)
		{
			author = document.DocumentNode.SelectNodes("//div[contains(@class, \"author\")]").FirstOrDefault()?.InnerText;
			if (string.IsNullOrWhiteSpace(author))
			{
				return false;
			}

			author = HttpUtility.HtmlDecode(author.Replace("by ", string.Empty));
			return true;
		}

		private bool TryGetMapPreviewUrl(HtmlDocument document, string number, out string imageUrl)
		{
			var imageUrlNode = document.DocumentNode.SelectNodes("//img[contains(@alt, \"minimap\")]").SingleOrDefault(x => x.Attributes["src"].Value == $"/maps/{number}/minimap");
			imageUrl = imageUrlNode?.Attributes["src"]?.Value;
			if (string.IsNullOrWhiteSpace(imageUrl))
			{
				return false;
			}

			imageUrl = $"{BaseUrl}{imageUrl}";
			return true;
		}

		private bool TryGetTargetMod(HtmlDocument document, out string targetModIdentifier, out string targetModName)
		{
			var targetModNode = document.DocumentNode.Descendants().SingleOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "span" && n.GetAttributeValue("class", string.Empty).StartsWith("mod_"));
			targetModIdentifier = targetModNode?.GetAttributeValue("class", null);
			targetModName = targetModNode?.InnerText;
			return !string.IsNullOrWhiteSpace(targetModIdentifier) && !string.IsNullOrWhiteSpace(targetModName);
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

			var hexColor = response.Content.Substring(response.Content.IndexOf(modIdentifier, StringComparison.Ordinal));
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
			return playersNode?.NextSibling.InnerText;
		}

		private string GetMapSize(HtmlDocument document)
		{
			var sizeNode = document.DocumentNode.Descendants()
				.SingleOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "div"
																		 && n.GetAttributeValue("class", string.Empty) == "map_data_label"
																		 && n.InnerText == "Size:");
			return sizeNode?.NextSibling.InnerText;
		}

		private string GetPublishedDate(HtmlDocument document)
		{
			var publishedNode = document.DocumentNode.Descendants()
				.SingleOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "div"
																		 && n.GetAttributeValue("class", string.Empty) == "map_data_label"
																		 && n.InnerText == "Published:");
			return publishedNode?.NextSibling.InnerText;
		}

		#endregion
	}
}
