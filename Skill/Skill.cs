using System.Collections.Generic;
using UnityEngine;

internal abstract class Skill
{
    public bool EnableIndicator { get; protected set; }
    private List<Effect> _effects;
    private ISkillIndicator _skillIndicator;
    
    public void Initialize(SkillTable.Row skillTable)
    {
        CreateEffects(skillTable);
        CreateSkillIndicator(skillTable);
    }

    public virtual void OnUpdate()
    {
        _effects.ForEach(_ => _.OnUpdate());
        _skillIndicator?.OnUpdate();
    }
    
    public virtual void Apply(IAttacker owner)
    {
        HandleRotationAndIndicator(owner);
        _effects.ForEach(x => x.Apply(owner, null));
    }

    public void ShowIndicator()
    {
        _skillIndicator?.Show();
    }
    
    private void CreateEffects(SkillTable.Row skillTable)
    {
        var effectIds = skillTable.EffectIds;
        var count = effectIds.Count;
        if (count == 0)
        {
            return;
        }
        
        _effects = new List<Effect>(count);
        foreach (var effectId in effectIds)
        {
            var effectTable = TableManager.GetTable<EffectTable>().GetRow(effectId);
            var effect = EffectFactory.Create(effectTable.EffectType);
            effect.Initialize(effectTable);
            _effects.Add(effect);
        }
    }
    
    private void CreateSkillIndicator(SkillTable.Row skillTable)
    {
        var skillShapeType = skillTable.SkillShapeType;
        if (skillShapeType == eSkillShape.None)
        {
            return;
        }
        
        _skillIndicator = SkillIndicatorFactory.Create(skillShapeType);
    }

    private void HandleRotationAndIndicator(IAttacker owner)
    {
        if (_skillIndicator == null)
        {
            return;
        }
        
        owner.Tm.rotation = Quaternion.LookRotation(_skillIndicator.Direction);
        _skillIndicator.Hide();
    }
}