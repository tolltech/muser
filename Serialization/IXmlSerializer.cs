﻿namespace Tolltech.Serialization
{
    public interface IXmlSerializer
    {
        byte[] Serialize(object data);
        byte[] Serialize<T>(T data);
        T Deserialize<T>(byte[] bytes);
        string SerializeToString(object data);
        T DeserializeFromString<T>(string data);
    }
}