using Newtonsoft.Json;
using System;

namespace Tests.Helpers
{
    internal class DateISOConverter : JsonConverter<DateTime>
    {
        public string DateTimeFormat = @"yyyy-MM-dd";

        public DateISOConverter() { }

        public DateISOConverter(string datetimeFormat)
        {
            DateTimeFormat = datetimeFormat;
        }

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            var timespanFormatted = $"{value.ToString(DateTimeFormat)}";
            writer.WriteValue(timespanFormatted);
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return DateTime.ParseExact((string)reader.Value, DateTimeFormat, null);
        }
    }
}