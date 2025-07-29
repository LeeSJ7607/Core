using System.Collections.Generic;

internal sealed class BuffController
{
    private readonly Dictionary<eBuffEffect, List<Buff>> _buffMap = new();

    public void OnUpdate()
    {
        foreach (var buffs in _buffMap.Values)
        {
            UpdateAndCleanBuffs(buffs);
        }
    }

    private void UpdateAndCleanBuffs(IList<Buff> buffs)
    {
        for (var i = buffs.Count - 1; i >= 0; i--)
        {
            var buff = buffs[i];
            buff.OnUpdate();

            if (buff.IsExpired())
            {
                buffs.RemoveAt(i);
            }
        }
    }
    
    public void AddBuff(Buff buff)
    {
        if (TryGetOrAddBuff(buff, out var existingBuff))
        {
            existingBuff.HandleOverlap();
        }
    }
    
    private bool TryGetOrAddBuff(Buff buff, out Buff existingBuff)
    {
        var buffTable = buff.BuffTable;
        var buffEffectType = buffTable.BuffEffectType;

        if (!_buffMap.TryGetValue(buffEffectType, out var refBuffs))
        {
            existingBuff = null;
            _buffMap.Add(buffEffectType, new List<Buff>() { buff });
            return false;
        }

        existingBuff = refBuffs.Find(x => x.BuffTable.Id == buffTable.Id);
        if (existingBuff == null)
        {
            refBuffs.Add(buff);
            return false;
        }

        return true;
    }
}