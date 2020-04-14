using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Tolltech.Serialization
{
    public class XMLSerializer : IXmlSerializer
    {
        private readonly ConcurrentDictionary<Type, XmlSerializer> serializers = new ConcurrentDictionary<Type, XmlSerializer>();
        private readonly XmlWriterSettings xmlWriterSettings;
        private readonly XmlSerializerNamespaces ns;
        private readonly Func<Type, XmlSerializer> createSerializerFunc = type => new XmlSerializer(type);

        public XMLSerializer()
        {
            xmlWriterSettings = new XmlWriterSettings
                                    {
                                        Indent = true,
                                        Encoding = new UTF8Encoding(false),
                                        OmitXmlDeclaration = true
                                    };

            ns = new XmlSerializerNamespaces();
            ns.Add("", "");
        }

        private byte[] Serialize(object obj, Type objType)
        {
            var serializer = serializers.GetOrAdd(objType, createSerializerFunc);
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, xmlWriterSettings))
                {
                    serializer.Serialize(writer, obj, ns);
                }
                return stream.ToArray();
            }
        }

        private T Deserialize<T>(System.IO.Stream stream)
        {
            return (T)Deserialize(stream, typeof(T));
        }

        public byte[] Serialize(object data)
        {
            return Serialize(data, data.GetType());
        }

        public byte[] Serialize<T>(T data)
        {
            return Serialize(data, typeof(T));
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return (T)Deserialize(bytes, typeof(T));
        }

        public string SerializeToString(object data)
        {
            return Encoding.UTF8.GetString(Serialize(data, data.GetType()));
        }

        public T DeserializeFromString<T>(string data)
        {
            return (T)Deserialize(Encoding.UTF8.GetBytes(data), typeof(T));
        }

        private object Deserialize(byte[] bytes, Type objType)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Deserialize(stream, objType);
            }
        }

        private object Deserialize(Stream stream, Type objType)
        {
            var serializer = serializers.GetOrAdd(objType, createSerializerFunc);
            return serializer.Deserialize(stream);
        }
    }
}