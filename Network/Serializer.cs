using System;
using MemoryPack;

internal sealed class Serializer
{
    public byte[] Serialize(IRequest request)
    {
        return MemoryPackSerializer.Serialize(request);
    }

    public object Deserialize(Type type, byte[] buffer)
    {
        return MemoryPackSerializer.Deserialize(type, buffer);
    }
}