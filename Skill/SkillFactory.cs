using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class SkillFactory
{
    private static readonly Dictionary<eSkillInput, Func<Skill>> _skillMap = new()
    {
        [eSkillInput.Instant] = () => new InstantSkill(),
        [eSkillInput.TargetUnit] = () => new TargetUnitSkill(),
        [eSkillInput.Directional] = () => new DirectionalSkill(),
        [eSkillInput.GroundPoint] = () => new GroundPointSkill(),
    };
    
    public static Skill GetSkill(eSkillInput skillInputType)
    {
        if (_skillMap.TryGetValue(skillInputType, out var skill))
        {
            return skill.Invoke();
        }

        Debug.LogError($"[{nameof(GetSkill)}] Skill not found for state: {skillInputType}");
        return _skillMap[eSkillInput.Instant].Invoke();
    }
}