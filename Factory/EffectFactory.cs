using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class EffectFactory
{
    private static readonly Dictionary<eEffectType, Func<Effect>> _effectMap = new()
    {
        [eEffectType.Buff] = () => new BuffEffect(),
        [eEffectType.Pull] = () => new PullingEffect(),
    };

    public static Effect GetEffect(eEffectType effectType)
    {
        if (_effectMap.TryGetValue(effectType, out var effect))
        {
            return effect.Invoke();
        }
        
        Debug.LogError($"[{nameof(GetEffect)}] Effect not found for state: {effectType}");
        return _effectMap[eEffectType.Buff].Invoke();
    }
}