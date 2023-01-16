using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Carpeddit.Common.Converters
{
    public abstract class BaseTimestampConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var valueString = reader.GetDouble().ToString();

                if (valueString.Length > 0 && !bool.TryParse(valueString, out _))
                {
                    if (DateTime.TryParse(valueString, out DateTime parsedDate))
                        return parsedDate;

                    return ParseDateFromSeconds((long)Convert.ToDouble(valueString));
                }
            } catch
            {

            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value is DateTime date && date != default)
            {
                writer.WriteRawValue(ConvertToSeconds(date).ToString());
                return;
            }

            writer.WriteNullValue();
        }

        public abstract long ConvertToSeconds(DateTime dateTime);

        public abstract DateTime ParseDateFromSeconds(long seconds);
    }
}
