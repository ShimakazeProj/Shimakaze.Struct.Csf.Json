using Shimakaze.Struct.Csf.Helper;

using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shimakaze.Struct.Csf.Json
{
    public class CsfLabelJsonConverter : JsonConverter<CsfLabel>
    {
        private CsfStringJsonConverter strcvtr = new CsfStringJsonConverter();
        public override CsfLabel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 获取属性名
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("TokenType are not a PropertyName");
            CsfLabel csflbl = new CsfLabel
            {
                Name = reader.GetString()
            };
            reader.Read();
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var sb = new StringBuilder();
                bool useStringBuilder = false;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        if (useStringBuilder)
                            csflbl.Add(CsfStringHelper.Create(sb.ToString()));
                        break;
                    }
                    else if (reader.TokenType == JsonTokenType.String)
                    {
                        useStringBuilder = true;
                        sb.AppendLine(reader.GetString());
                    }
                    else if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        csflbl.Add(strcvtr.Read(ref reader, typeToConvert, options));
                    }
                }
                return csflbl;
            }
            else if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.StartObject)
            {
                csflbl.Add(strcvtr.Read(ref reader, typeToConvert, options));
                return csflbl;
            }
            else throw new JsonException();
        }
        public override void Write(Utf8JsonWriter writer, CsfLabel value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.Name);
            if (value.Count > 1)
            {
                writer.WriteStartArray();
            }
            foreach (var item in value)
            {
                if (value.Count > 1)
                {
                    strcvtr.Write(writer, item, true);
                }
                else strcvtr.Write(writer, item, options);
            }

            if (value.Count > 1)
            {
                writer.WriteEndArray();
            }
        }
    }
}
