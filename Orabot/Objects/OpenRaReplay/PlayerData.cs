using System;
using System.Collections.Generic;
using System.Text;

namespace Orabot.Objects.OpenRaReplay
{
	public class PlayerData
	{
		public int ClientIndex { get; set; }

		public string Name { get; set; }

		public bool IsHuman { get; set; }

		public bool IsBot { get; set; }

		public string FactionName { get; set; }

		public string FactionId { get; set; }

		public string Color { get; set; }

		public int Team { get; set; }

		public int SpawnPoint { get; set; }

		public bool IsRandomFaction { get; set; }

		public bool IsRandomSpawnPoint { get; set; }

		public string Fingerprint { get; set; }

		public string Outcome { get; set; }

		public string OutcomeTimestampUtc { get; set; }
	}
}
