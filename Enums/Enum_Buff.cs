using System;

public enum eBuffType
{
    Buff,
    DeBuff,
}

public enum eBuffOverlap
{
    Ignore,
    Stack,
    Refresh,
}

[Flags]
public enum eDeBuffStatus
{
    None = 0,
    Stun = 1 << 0,
    Poison = 1 << 1,
    Pulling = 1 << 2,
}