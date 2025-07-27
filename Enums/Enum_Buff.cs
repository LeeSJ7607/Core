using System;

[Flags]
public enum eBuffEffect
{
    None = 0,
    Stun = 1 << 0,
    Poison = 1 << 1,
}

public enum eBuffOverlap
{
    Ignore,
    Reset,
    StackNoRefresh,
    StackAndRefresh,
    RefreshOnly,
}

public enum eBuffStack
{
    Additive,
    Multiplicative,
}

//TODO: 버프 우선 순위