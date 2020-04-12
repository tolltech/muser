using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Tolltech.Serialization
{
    public class JsonSerializer : IJsonSerializer
    {

        public byte[] Serialize(object data)
        {
            var jsonStr = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(jsonStr);
        }

        public T Deserialize<T>(byte[] data)
        {
            var bytes = Encoding.UTF8.GetString(data);
            if (string.IsNullOrEmpty(bytes))
                throw new SerializationException("Fail to deserialize object from empty bytes array.");

            return (T)JsonConvert.DeserializeObject(bytes, typeof(T), new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
        }

        public string SerializeToString(object data)
        {
            return Encoding.UTF8.GetString(Serialize(data));
        }

        public T DeserializeFromString<T>(string data)
        {
            return Deserialize<T>(Encoding.UTF8.GetBytes(data));
        }
    }
}