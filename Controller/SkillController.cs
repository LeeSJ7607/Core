using System.Collections.Generic;

public sealed class SkillController
{
    private readonly Dictionary<eSkillSlot, Skill> _skillBySlotMap = new();

    public void OnUpdate()
    {
        foreach (var skill in _skillBySlotMap.Values)
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
            var skill = SkillFactory.Create(skillTable.SkillInputType);
            skill.Initialize(skillTable);
            _skillBySlotMap.Add(slotType, skill);
        }
    }

    public void ShowIndicator(IAttacker owner, eSkillSlot slotType)
    {
        if (!_skillBySlotMap.TryGetValue(slotType, out var skill))
        {
            return;
        }
        
        skill.ShowIndicator();
    }

    public void ExecuteSkill(IAttacker owner)
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
        foreach (var skill in _skillBySlotMap.Values)
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