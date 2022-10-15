using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Orabot.Core.Objects.OpenRaReplay;
using YamlDotNet.Serialization;

namespace Orabot.Core.Transformers.Replays.ReplayToReplayDataTransformers
{
	internal class AttachmentReplayToUtilityMetadataTransformer
	{
		private readonly string _openRaUtilityPath;
		private readonly string _replayStorageFolder;

		private readonly IDeserializer _yamlDeserializer;

		public AttachmentReplayToUtilityMetadataTransformer(IDeserializer yamlDeserializer, IConfiguration configuration)
		{
			_yamlDeserializer = yamlDeserializer;
			_openRaUtilityPath = configuration["OpenRaUtilityPath"];
			_replayStorageFolder = configuration["ReplayStorageFolder"];
		}

		internal ReplayMetadata GetMetadata(Discord.Attachment attachment)
		{
			var filePath = Path.Combine(_replayStorageFolder, $"{Guid.NewGuid()}_{attachment.Filename}");

			using var webClient = new WebClient();
			webClient.DownloadFile(attachment.Url, filePath);

			var output = GetUtilityOutput(filePath);

			return _yamlDeserializer.Deserialize<ReplayMetadata>(output);
		}

		#region Private methods

		private string GetUtilityOutput(string filePath)
		{
			using var process = new Process
			{
				StartInfo = GetProcessStartInfo(filePath)
			};

			process.Start();

			// Synchronously read the standard output of the spawned process.
			var reader = process.StandardOutput;
			var output = reader.ReadToEnd();

			process.WaitForExit();

			output = output.Substring(output.IndexOf("\n", StringComparison.Ordinal) + 1);
			output = output.Replace("\t", "  ");
			output = output.Replace("{DEV_VERSION}", "DEV_VERSION");

			return output;
		}

		private ProcessStartInfo GetProcessStartInfo(string filePath)
		{
			string fileName;
			string arguments;

			// Workaround to mono problems inspired by https://github.com/mono/mono/issues/17204#issuecomment-697329095
			// This will likely go away once OpenRA switches to .NET 5.
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				fileName = _openRaUtilityPath;
				arguments = $"d2k --replay-metadata \"{filePath}\"";
			}
			else
			{
				fileName = "mono";
				arguments = $"{_openRaUtilityPath} d2k --replay-metadata \"{filePath}\"";
			}

			return new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = arguments,
				UseShellExecute = false,
				RedirectStandardOutput = true
			};
		}

		#endregion
	}
}
