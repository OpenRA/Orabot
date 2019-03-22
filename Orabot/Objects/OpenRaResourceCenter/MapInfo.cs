using System;
using System.Collections.Generic;
using RestSharp.Deserializers;

namespace Orabot.Objects.OpenRaResourceCenter
{
	public class MapInfo
	{
		[DeserializeAs(Name = "categories")]
		public List<string> Categories { get; set; }

		[DeserializeAs(Name = "width")]
		public string Width { get; set; }

		[DeserializeAs(Name = "revision")]
		public int Revision { get; set; }

		[DeserializeAs(Name = "title")]
		public string Title { get; set; }

		[DeserializeAs(Name = "mapformat")]
		public int MapFormat { get; set; }

		[DeserializeAs(Name = "rating")]
		public double Rating { get; set; }

		[DeserializeAs(Name = "spawnpoints")]
		public string SpawnPoints { get; set; }

		[DeserializeAs(Name = "lua")]
		public bool HasLua { get; set; }

		[DeserializeAs(Name = "bounds")]
		public string Bounds { get; set; }

		[DeserializeAs(Name = "map_grid_type")]
		public string MapGridType { get; set; }

		[DeserializeAs(Name = "height")]
		public string Height { get; set; }

		[DeserializeAs(Name = "reports")]
		public int Reports { get; set; }

		[DeserializeAs(Name = "parser")]
		public string Parser { get; set; }

		[DeserializeAs(Name = "author")]
		public string Author { get; set; }

		[DeserializeAs(Name = "players_block")]
		public string PlayersBlock { get; set; }

		[DeserializeAs(Name = "requires_upgrade")]
		public bool RequiresUpgrade { get; set; }

		[DeserializeAs(Name = "downloaded")]
		public int Downloaded { get; set; }

		[DeserializeAs(Name = "id")]
		public int Id { get; set; }

		[DeserializeAs(Name = "downloading")]
		public bool IsDownloadingEnabled { get; set; }

		[DeserializeAs(Name = "url")]
		public string Url { get; set; }

		[DeserializeAs(Name = "last_revision")]
		public bool LastRevision { get; set; }

		[DeserializeAs(Name = "viewed")]
		public int Viewed { get; set; }

		[DeserializeAs(Name = "players")]
		public int Players { get; set; }

		[DeserializeAs(Name = "rules")]
		public string Rules { get; set; }

		[DeserializeAs(Name = "minimap")]
		public string Minimap { get; set; }

		[DeserializeAs(Name = "info")]
		public string Info { get; set; }

		[DeserializeAs(Name = "game_mod")]
		public string GameMod { get; set; }

		[DeserializeAs(Name = "tileset")]
		public string Tileset { get; set; }

		[DeserializeAs(Name = "map_hash")]
		public string MapHash { get; set; }

		[DeserializeAs(Name = "advanced_map")]
		public bool IsAdvancedMap { get; set; }

		[DeserializeAs(Name = "posted")]
		public DateTime PostedOn { get; set; }

		[DeserializeAs(Name = "description")]
		public string Description { get; set; }

		[DeserializeAs(Name = "license")]
		public string License { get; set; }

		[DeserializeAs(Name = "map_type")]
		public string MapType { get; set; }
	}
}
