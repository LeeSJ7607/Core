using System;

public enum eBuffCategory
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
public enum eBuffEffect
{
    None = 0,
    Stun = 1 << 0,
    Poison = 1 << 1,
    Pulling = 1 << 2,
}