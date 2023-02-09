using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Carpeddit.Common.Converters
{
    public abstract class EmptyStringToListConverter<T> : JsonConverter<IEnumerable<T>>
    {
        public override IEnumerable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();

            if (string.IsNullOrEmpty(str))
                return Enumerable.Empty<T>();

            return JsonSerializer.Deserialize<IEnumerable<T>>(Encoding.UTF8.GetString(reader.ValueSpan.ToArray()));
        }
    }
}
