using System.Collections.Generic;

internal abstract class Skill
{
    public bool EnableIndicator { get; protected set; }
    private SkillTable.Row _skillTable;
    private List<Effect> _effects;
    
    public void Initialize(SkillTable.Row skillTable)
    {
        _skillTable = skillTable;
        CreateEffects();
    }
    
    public virtual void OnUpdate() { }
    public virtual void ShowIndicator() { }
    public virtual void HideIndicator() { }
    public virtual void Apply(IReadOnlyUnit owner)
    {
        if (_effects.IsNullOrEmpty())
        {
            return;
        }
        
        foreach (var effect in _effects)
        {
            effect.Apply(owner, null);
        }
    }
    
    private void CreateEffects()
    {
        var effectIds = _skillTable.EffectIds;
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
}