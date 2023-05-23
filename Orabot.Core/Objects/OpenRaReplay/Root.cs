using System;

namespace Orabot.Core.Objects.OpenRaReplay
{
	public class Root
	{
		public string Mod { get; set; }

		public string Version { get; set; }

		public string MapUid { get; set; }

		public string MapTitle { get; set; }

		public int FinalGameTick { get; set; }

		public DateTime StartTimeUtc { get; set; }

		public DateTime EndTimeUtc { get; set; }

		public string DisabledSpawnPoints { get; set; } // TODO: probably wrong
	}
}
