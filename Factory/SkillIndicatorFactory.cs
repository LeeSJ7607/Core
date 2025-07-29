using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class SkillIndicatorFactory
{
    private static readonly Dictionary<eSkillShape, Func<ISkillIndicator>> _indicatorMap = new()
    {
        [eSkillShape.LineProjectile] = () => new SkillLineIndicator(),
    };

    public static ISkillIndicator Create(eSkillShape skillShape)
    {
        if (_indicatorMap.TryGetValue(skillShape, out var indicator))
        {
            return indicator.Invoke();
        }
        
        Debug.LogError($"[{nameof(Create)}] Skill indicator not found for shape: {skillShape}");
        return _indicatorMap[eSkillShape.LineProjectile].Invoke();
    }
}