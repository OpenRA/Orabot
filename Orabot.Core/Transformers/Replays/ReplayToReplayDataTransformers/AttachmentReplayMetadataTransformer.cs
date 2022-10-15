﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Orabot.Core.Objects.OpenRaReplay;
using YamlDotNet.Serialization;

namespace Orabot.Core.Transformers.Replays.ReplayToReplayDataTransformers
{
	internal class AttachmentReplayMetadataTransformer
	{
		const int MetaEndMarker = -2;

		private readonly string _replayStorageFolder;

		private readonly IDeserializer _yamlDeserializer;

		public AttachmentReplayMetadataTransformer(IDeserializer yamlDeserializer, IConfiguration configuration)
		{
			_replayStorageFolder = configuration["ReplayStorageFolder"];
			_yamlDeserializer = yamlDeserializer;
		}

		internal ReplayMetadata GetMetadata(Discord.Attachment attachment)
		{
			var filePath = Path.Combine(_replayStorageFolder, $"{Guid.NewGuid()}_{attachment.Filename}");

			using var webClient = new WebClient();
			webClient.DownloadFile(attachment.Url, filePath);

			var yaml = ExtractYaml(filePath);

			var splitYaml = yaml.Split("Player:");
			var rootYaml = splitYaml[0];

			var metadata = _yamlDeserializer.Deserialize<ReplayData>(rootYaml);
			var players = new Dictionary<int, PlayerData>();

			var p = 0;
			foreach (var playerYaml in splitYaml.Skip(1))
			{
				var player = _yamlDeserializer.Deserialize<PlayerData>(playerYaml);
				players.Add(p, player);
			}

			var replayMetadata = new ReplayMetadata()
			{
				FileName = attachment.Filename,
				Mod = metadata.Root.Mod,
				Version = metadata.Root.Version,
				MapUid = metadata.Root.MapUid ,
				MapTitle = metadata.Root.MapTitle,
				FinalGameTick = metadata.Root.FinalGameTick,
				StartTimeUtc = metadata.Root.StartTimeUtc,
				EndTimeUtc = metadata.Root.EndTimeUtc,
				Players = players
			};

			return replayMetadata;
		}

		#region Private methods

		private string ExtractYaml(string filePath)
		{
			var fileStream = new FileStream(filePath, FileMode.Open);
			var miniYaml = ExtractMetaData(fileStream);
			var yaml = Regex.Replace(miniYaml.Replace("\t", "  "), @"@\d+", "");
			return yaml.Replace("{DEV_VERSION}", "development");
		}

		private static string ExtractMetaData(FileStream fileStream)
		{
			try
			{
				if (!fileStream.CanSeek)
					throw new InvalidOperationException("Can't seek stream.");

				if (fileStream.Length < 20)
					throw new InvalidDataException("File too short.");

				fileStream.Seek(-(4 + 4), SeekOrigin.End);
				var dataLength = fileStream.ReadInt32();
				if (fileStream.ReadInt32() == MetaEndMarker)
				{
					// Go back by (end marker + length storage + data + version + start marker) bytes
					fileStream.Seek(-(4 + 4 + dataLength + 4 + 4), SeekOrigin.Current);

					var unknown1 = fileStream.ReadInt32();
					var unknown2 = fileStream.ReadInt32();
					var unknown3 = fileStream.ReadInt32();

					using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
					var metadata = streamReader.ReadToEnd();
					metadata = metadata.Remove(metadata.Length - 8);
					return metadata;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}

			return null;
		}

		#endregion
	}
}
