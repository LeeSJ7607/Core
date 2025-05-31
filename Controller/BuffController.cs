using System.Collections.Generic;

internal sealed class BuffController
{
    private readonly Dictionary<eBuffEffect, List<Buff>> _buffMap = new(); //TODO: 중복을 허용하지 않으면 해쉬셋으로 교체.
    
    //TODO: 사용하지 않으면 삭제.
    public BuffController(IReadOnlyUnit owner)
    {
        
    }

    public void AddBuff(Buff buff)
    {
        
    }
}