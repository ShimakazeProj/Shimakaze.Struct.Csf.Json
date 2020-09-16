using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Shimakaze.Struct.Csf.Helper;

namespace Shimakaze.Struct.Csf.Json
{
    public class CsfStringJsonConverter : JsonConverter<CsfString>
    {
        public override CsfString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            CsfString csfStr = new CsfString();
            // 不是一个对象开始则抛出异常
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject) return csfStr;

                    // 获取属性名
                    if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("TokenType are not a PropertyName");
                    string propertyName = reader.GetString().ToLower();

                    if (propertyName.Equals("value"))
                    {
                        reader.Read();
                        readValue(ref reader, csfStr);
                    }
                    else if (propertyName.Equals("extra"))
                    {
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.String)
                            csfStr = csfStr.ToWString(reader.GetString());
                    }
                }
            }
            else readValue(ref reader, csfStr);
            return csfStr;

            void readValue(ref Utf8JsonReader jsonReader, CsfString csf)
            {
                if (jsonReader.TokenType == JsonTokenType.String)
                {
                    csf.Content = jsonReader.GetString();
                }
                else if (jsonReader.TokenType == JsonTokenType.StartArray)
                {
                    var sb = new StringBuilder();
                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType == JsonTokenType.EndArray) break;
                        if (jsonReader.TokenType == JsonTokenType.String)
                            sb.AppendLine(jsonReader.GetString());
                    }
                    csf.Content = sb.ToString();
                }
                else throw new JsonException();
            }
        }
        public override void Write(Utf8JsonWriter writer, CsfString value, JsonSerializerOptions options)
        {
            this.Write(writer, value, false);
        }
        public void Write(Utf8JsonWriter writer, CsfString value, bool forceKeyValue)
        {
            if (value.IsWString || forceKeyValue)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("value");
            }
            writeValue(writer, value);
            if (value is CsfWString wstr)
            {
                writer.WriteString("extra", wstr.Extra);
            }
            if (value.IsWString || forceKeyValue)
                writer.WriteEndObject();
        }

        private static void writeValue(Utf8JsonWriter writer, CsfString value)
        {
            if (value.Content.Split('\n').Length > 1)
            {
                writer.WriteStartArray();
                {
                    using (var sr = new StringReader(value.Content))
                        while (sr.Peek() > 0)
                            writer.WriteStringValue(sr.ReadLine());
                }
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteStringValue(value.Content);
            }
        }
    }
}
