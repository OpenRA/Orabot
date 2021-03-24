using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace Orabot.Transformers.AttachmentToMessageTransformers
{
	internal class AttachmentLogFileToMessageTransformer
	{
		private static readonly string LogStorageFolder = ConfigurationManager.AppSettings["LogStorageFolder"];

		internal string CreateMessage(Discord.Attachment attachment)
		{
			var filePath = Path.Combine(LogStorageFolder, $"{Guid.NewGuid()}_{attachment.Filename}");

			using var webClient = new WebClient();
			webClient.DownloadFile(attachment.Url, filePath);

			var text = File.ReadAllText(filePath);

			return $"```{text.Substring(0, Math.Min(1000, text.Length))}```";
		}
	}
}
