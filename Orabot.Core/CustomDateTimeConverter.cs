using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Orabot.Core
{
	public class CustomDateTimeConverter : JsonConverter<DateTime>
	{
		public override DateTime Read(
			ref Utf8JsonReader reader,
			Type typeToConvert,
			JsonSerializerOptions options)
		{
			var rawValue = reader.GetString();

			// This is the format we get from the OpenRA Resource Center for maps' `posted_on` field.
			if (DateTime.TryParseExact(rawValue, "yyyy-MM-dd HH:mm:ss.ffffffK", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
				return result;

			// This is the format that GitHub's API gives us for PR/issue created_at/updated_at/etc.
			if (DateTime.TryParseExact(rawValue, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InstalledUICulture, DateTimeStyles.None, out result))
				return result;

			return DateTime.TryParse(rawValue, out result) ? result : DateTime.MinValue;
		}

		public override void Write(
			Utf8JsonWriter writer,
			DateTime dateTimeValue,
			JsonSerializerOptions options) => writer.WriteStringValue(dateTimeValue.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
	}
}
