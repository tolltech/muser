namespace Tolltech.Serialization
{
    public interface IJsonSerializer
    {
        byte[] Serialize(object data);
        T Deserialize<T>(byte[] data);
        string SerializeToString(object data);
        T DeserializeFromString<T>(string data);
    }
}