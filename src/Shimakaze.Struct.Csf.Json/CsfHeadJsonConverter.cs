using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shimakaze.Struct.Csf.Json
{
    public class CsfHeadJsonConverter : JsonConverter<CsfHead>
    {
        public override CsfHead Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 不是一个对象开始则抛出异常
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("TokenType are not a StartObject");

            CsfHead head = new CsfHead();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) return head;

                // 获取属性名
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("TokenType are not a PropertyName");
                string propertyName = reader.GetString().ToLower();

                if (propertyName.Equals("version"))
                {
                    reader.Read();
                    if (reader.TokenType == JsonTokenType.Number)
                        head.Version = reader.GetInt32();
                }
                else if (propertyName.Equals("language"))
                {
                    reader.Read();
                    if (reader.TokenType == JsonTokenType.Number)
                        head.Language = reader.GetInt32();
                }
                else if (propertyName.Equals("message"))
                {
                    reader.Read();
                    if (reader.TokenType == JsonTokenType.Number)
                        head.Unknown = reader.GetInt32();
                }
            }

            throw new JsonException();
        }
        public override void Write(Utf8JsonWriter writer, CsfHead value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            {
                writer.WriteNumber("version", value.Version);
                writer.WriteNumber("language", value.Language);
                if (value.Unknown > 0)
                    writer.WriteNumber("message", value.Unknown);
            }
            writer.WriteEndObject();
        }
    }
}
