﻿using MemoryPack;

public interface IResponse
{
    
}

[MemoryPackable]
public partial class LoginRes : IResponse
{
    public eLoginResult ResultType;
}