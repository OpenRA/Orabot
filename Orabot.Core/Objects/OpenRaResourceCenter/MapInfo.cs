using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Orabot.Core.Objects.OpenRaResourceCenter
{
	public class MapInfo
	{
		[JsonPropertyName("categories")]
		public List<string> Categories { get; set; }

		[JsonPropertyName("width")]
		public string Width { get; set; }

		[JsonPropertyName("revision")]
		public int Revision { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("mapformat")]
		public int MapFormat { get; set; }

		[JsonPropertyName("rating")]
		public double Rating { get; set; }

		[JsonPropertyName("spawnpoints")]
		public string SpawnPoints { get; set; }

		[JsonPropertyName("lua")]
		public bool HasLua { get; set; }

		[JsonPropertyName("bounds")]
		public string Bounds { get; set; }

		[JsonPropertyName("map_grid_type")]
		public string MapGridType { get; set; }

		[JsonPropertyName("height")]
		public string Height { get; set; }

		[JsonPropertyName("reports")]
		public int Reports { get; set; }

		[JsonPropertyName("parser")]
		public string Parser { get; set; }

		[JsonPropertyName("author")]
		public string Author { get; set; }

		[JsonPropertyName("players_block")]
		public string PlayersBlock { get; set; }

		[JsonPropertyName("requires_upgrade")]
		public bool RequiresUpgrade { get; set; }

		[JsonPropertyName("downloaded")]
		public int Downloaded { get; set; }

		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("downloading")]
		public bool IsDownloadingEnabled { get; set; }

		[JsonPropertyName("url")]
		public string Url { get; set; }

		[JsonPropertyName("last_revision")]
		public bool LastRevision { get; set; }

		[JsonPropertyName("viewed")]
		public int Viewed { get; set; }

		[JsonPropertyName("players")]
		public int Players { get; set; }

		[JsonPropertyName("rules")]
		public string Rules { get; set; }

		[JsonPropertyName("minimap")]
		public string Minimap { get; set; }

		[JsonPropertyName("info")]
		public string Info { get; set; }

		[JsonPropertyName("game_mod")]
		public string GameMod { get; set; }

		[JsonPropertyName("tileset")]
		public string Tileset { get; set; }

		[JsonPropertyName("map_hash")]
		public string MapHash { get; set; }

		[JsonPropertyName("advanced_map")]
		public bool IsAdvancedMap { get; set; }

		[JsonPropertyName("posted")]
		public DateTime PostedOn { get; set; }

		[JsonPropertyName("description")]
		public string Description { get; set; }

		[JsonPropertyName("license")]
		public string License { get; set; }

		[JsonPropertyName("map_type")]
		public string MapType { get; set; }
	}
}
