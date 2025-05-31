using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class BuffFactory
{
    private static readonly Dictionary<eBuffEffect, Func<Buff>> _buffMap = new()
    {
        [eBuffEffect.Stun] = () => new StunBuff(),
        [eBuffEffect.Poison] = () => new PosionBuff(),
    };

    public static Buff GetBuff(eBuffEffect buffEffectType)
    {
        if (_buffMap.TryGetValue(buffEffectType, out var buff))
        {
            return buff.Invoke();
        }

        Debug.LogError($"[{nameof(GetBuff)}] Buff not found for state: {buffEffectType}");
        return _buffMap[eBuffEffect.Stun].Invoke();
    }
}