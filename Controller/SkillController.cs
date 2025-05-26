using System.Collections.Generic;

public sealed class SkillController
{
    private readonly Dictionary<eSkillSlot, Skill> _skillMap = new();

    public void OnUpdate()
    {
        foreach (var skill in _skillMap.Values)
        {
            skill.OnUpdate();
        }
    }
    
    public void Initialize(IReadOnlyUnit owner)
    {
        var uniqueSkillIdsBySlot = owner.UnitTable.UniqueSkillIdsBySlot;

        foreach (var (slotType, skillId) in uniqueSkillIdsBySlot)
        {
            var skillTable = TableManager.GetTable<SkillTable>().GetRow(skillId);
            var skill = SkillFactory.GetSkill(skillTable.SkillInputType);
            skill.Initialize(skillTable);
            _skillMap.Add(slotType, skill);
        }
    }

    public void ShowIndicator(IReadOnlyUnit owner, eSkillSlot slotType)
    {
        if (!_skillMap.TryGetValue(slotType, out var skill))
        {
            return;
        }
        
        skill.ShowIndicator();
    }

    public void ExecuteSkill(IReadOnlyUnit owner)
    {
        if (!TryGetPreparedSkill(out var refSkill))
        {
            return;
        }
        
        refSkill.HideIndicator();
        refSkill.Apply(owner);
    }

    private bool TryGetPreparedSkill(out Skill refSkill)
    {
        foreach (var skill in _skillMap.Values)
        {
            if (skill.EnableIndicator)
            {
                refSkill = skill;
                return true;
            }
        }

        refSkill = null;
        return false;
    }
}