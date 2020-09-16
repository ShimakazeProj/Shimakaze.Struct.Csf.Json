using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shimakaze.Struct.Csf.Json
{
    public class CsfDocumentJsonConverter : JsonConverter<CsfDocument>
    {
        private CsfHeadJsonConverter headcvtr = new CsfHeadJsonConverter();
        private CsfLabelJsonConverter lblcvtr = new CsfLabelJsonConverter();
        public override CsfDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 不是一个对象开始则抛出异常
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("TokenType are not a StartObject");

            CsfDocument csfdoc = new CsfDocument();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) return csfdoc;

                // 获取属性名
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("TokenType are not a PropertyName");
                string propertyName = reader.GetString().ToLower();

                if (propertyName.Equals("head"))
                {
                    reader.Read();
                    csfdoc.Head = headcvtr.Read(ref reader, typeToConvert, options);
                }
                else if (propertyName.Equals("datas"))
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject) break;
                        if (reader.TokenType == JsonTokenType.StartObject) continue;
                        csfdoc.Add(lblcvtr.Read(ref reader, typeToConvert, options));
                    }
                    csfdoc.Head.LabelCount = csfdoc.Count;
                    csfdoc.Head.StringCount = csfdoc.Select(i => i.Count).Sum();
                }
            }

            throw new JsonException();

        }
        public override void Write(Utf8JsonWriter writer, CsfDocument value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            {
                writer.WritePropertyName("head");
                headcvtr.Write(writer, value.Head, options);
                writer.WritePropertyName("datas");
                writer.WriteStartObject();
                {
                    foreach (var item in value)
                        lblcvtr.Write(writer, item, options);
                }
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }


    }
}
