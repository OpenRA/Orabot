using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using Orabot.Objects.OpenRaReplay;
using YamlDotNet.Serialization;

namespace Orabot.Transformers.Replays.ReplayToReplayDataTransformers
{
	internal class AttachmentReplayToUtilityMetadataTransformer
	{
		private static readonly string ReplayStorageFolder = ConfigurationManager.AppSettings["ReplayStorageFolder"];
		private static readonly string OpenRaUtilityPath = ConfigurationManager.AppSettings["OpenRaUtilityPath"];

		private readonly Deserializer _yamlDeserializer;

		public AttachmentReplayToUtilityMetadataTransformer(Deserializer yamlDeserializer)
		{
			
			_yamlDeserializer = yamlDeserializer;
		}

		internal ReplayMetadata GetMetadata(Discord.Attachment attachment)
		{
			var filePath = Path.Combine(ReplayStorageFolder, $"{Guid.NewGuid()}_{attachment.Filename}");

			using var webClient = new WebClient();
			webClient.DownloadFile(attachment.Url, filePath);

			using var process = new Process
			{
				StartInfo = new ProcessStartInfo(OpenRaUtilityPath, $"d2k --replay-metadata \"{filePath}\"")
				{
					UseShellExecute = false,
					RedirectStandardOutput = true
				}
			};

			process.Start();

			// Synchronously read the standard output of the spawned process.
			var reader = process.StandardOutput;
			var output = reader.ReadToEnd();

			process.WaitForExit();

			output = output.Substring(output.IndexOf("\n", StringComparison.Ordinal) + 1);
			output = output.Replace("\t", "  ");
			output = output.Replace("{DEV_VERSION}", "DEV_VERSION");

			return _yamlDeserializer.Deserialize<ReplayMetadata>(output);
		}
	}
}
