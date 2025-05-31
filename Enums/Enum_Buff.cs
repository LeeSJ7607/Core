using System;

public enum eBuffCategory
{
    Buff,
    DeBuff,
}

[Flags]
public enum eBuffEffect
{
    None = 0,
    Stun = 1 << 0,
    Poison = 1 << 1,
}

public enum eBuffOverlap
{
    Ignore,          // 같은 버프가 이미 있으면 아무 처리 안 함 (중복 무시)
    StackAndRefresh, // 버프 스택 증가 + 남은 지속시간 리셋 (시간 갱신)
    StackNoRefresh,  // 버프 스택 증가 + 남은 지속시간 유지 (시간 유지)
    RefreshOnly,     // 스택은 변하지 않고, 지속시간만 갱신
    Replace,         // 기존 버프 완전 교체 (스택과 시간 모두 초기화 및 새 버프 적용)
}

public enum eBuffStack
{
    Additive,
    Multiplicative,
}

//TODO: 버프 우선 순위