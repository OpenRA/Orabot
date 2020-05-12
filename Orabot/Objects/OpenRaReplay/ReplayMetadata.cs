using System;
using System.Collections.Generic;
using System.Text;

namespace Orabot.Objects.OpenRaReplay
{
	public class ReplayMetadata
	{
		public string FileName { get; set; }

		public string Mod { get; set; }

		public string Version { get; set; }

		public string MapUid { get; set; }

		public string MapTitle { get; set; }

		public string FinalGameTick { get; set; }

		public string StartTimeUtc { get; set; }

		public string EndTimeUtc { get; set; }

		public IDictionary<int, PlayerData> Players { get; set; }
	}
}
