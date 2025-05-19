using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

internal sealed class ListJsonConverter<T> : JsonConverter
{
    private const string SEPARATOR = "|";
    
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is List<T> list)
        {
            writer.WriteValue(string.Join(SEPARATOR, list));
            return;
        }
        
        throw new JsonSerializationException($"Expected List<{typeof(T).Name}>, but got {value?.GetType().Name}.");
    }
    
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var str = reader.Value as string;
        if (str.IsNullOrEmpty())
        {
            return new List<T>();
        }
        
        return str.Split(SEPARATOR)
                  .Select(OnSelect)
                  .Where(i => i >= 0)
                  .ToList();

        int OnSelect(string s)
        {
            if (int.TryParse(s, out var i))
            {
                return i;
            }

            return -1;
        }
    }
    
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<T>);
    }
}