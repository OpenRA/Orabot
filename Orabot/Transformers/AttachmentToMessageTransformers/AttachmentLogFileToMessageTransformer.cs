using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;

namespace Orabot.Transformers.AttachmentToMessageTransformers
{
	internal class AttachmentLogFileToMessageTransformer
	{
		private static readonly string LogStorageFolder = ConfigurationManager.AppSettings["LogStorageFolder"];

		internal string CreateRawMessage(Discord.Attachment attachment, out string fullText)
		{
			var filePath = Path.Combine(LogStorageFolder, $"{Guid.NewGuid()}_{attachment.Filename}");

			using var webClient = new WebClient();
			webClient.DownloadFile(attachment.Url, filePath);

			fullText = File.ReadAllText(filePath);

			return $"```{fullText.Substring(0, Math.Min(1000, fullText.Length))}```";
		}

		internal bool TryCreateExceptionExplanationMessage(string text, out string explanationMessage)
		{
			explanationMessage = null;

			var lines = text.Split("\r\n");
			var exceptionLine = lines.LastOrDefault(x => x.Trim().StartsWith("Exception of type `"));
			if (exceptionLine == null || exceptionLine.IndexOf("`: ", StringComparison.Ordinal) < 0)
				return false;

			exceptionLine = exceptionLine.Substring(exceptionLine.IndexOf("`: ", StringComparison.Ordinal) + 3);

			explanationMessage = $"Now, I'm no expert, but I suspect your problem is *probably*  related to this:\r\n> **{exceptionLine}**";

			var pointsOfInterest = new List<string>();
			for (var i = 0; i < lines.Length && pointsOfInterest.Count < 5; i++)
			{
				if (!lines[i].StartsWith("   at "))
					continue;

				if (!IsOpenRAStackTraceLine(lines[i]))
					continue;

				pointsOfInterest.Add(ExtractPointOfInterest(lines[i]));
			}

			if (pointsOfInterest.Count > 0)
				explanationMessage = $"{explanationMessage}\r\n\r\nPoints of interest:\r\n```{string.Join("\r\n", pointsOfInterest)}```";

			return true;
		}

		private static bool IsOpenRAStackTraceLine(string line)
		{
			return line.StartsWith("   at OpenRA");
		}

		private static string ExtractPointOfInterest(string line)
		{
			try
			{
				line = line.Substring(line.IndexOf("   at ") + "   at ".Length);
				return line.EndsWith(")") ? line : line.Substring(0, line.IndexOf(") ") + 1);
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}
}
