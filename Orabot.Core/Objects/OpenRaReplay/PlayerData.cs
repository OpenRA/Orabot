using System;

namespace Orabot.Core.Objects.OpenRaReplay
{
	// Copied from OpenRA.Game.GameInformation.Player.
	public class PlayerData
	{
		#region Start-up information

		public int ClientIndex;

		/// <summary>The player name, not guaranteed to be unique.</summary>
		public string Name;
		public bool IsHuman;
		public bool IsBot;

		/// <summary>The faction's display name.</summary>
		public string FactionName;

		/// <summary>The faction ID, a.k.a. the faction's internal name.</summary>
		public string FactionId;
		public string Color;

		/// <summary>The faction (including Random, etc.) that was selected in the lobby.</summary>
		public string DisplayFactionName;
		public string DisplayFactionId;

		/// <summary>The team ID on start-up, or 0 if the player is not part of a team.</summary>
		public int Team;
		public int SpawnPoint;
		public int Handicap;

		/// <summary>True if the faction was chosen at random; otherwise, false.</summary>
		public bool IsRandomFaction;

		/// <summary>True if the spawn point was chosen at random; otherwise, false.</summary>
		public bool IsRandomSpawnPoint;

		/// <summary>Player authentication fingerprint for the OpenRA forum.</summary>
		public string Fingerprint;

		#endregion

		#region

		/// <summary>The game outcome for this player.</summary>
		public string Outcome;

		/// <summary>The time when this player won or lost the game.</summary>
		public DateTime OutcomeTimestampUtc;

		/// <summary>The frame at which this player disconnected.</summary>
		public int DisconnectFrame;

		#endregion

		public override string ToString()
		{
			return $"{Name} [{(FactionName == DisplayFactionName ? FactionName : $"{FactionName} (picked \"{DisplayFactionName}\")")}]";
		}
	}
}
