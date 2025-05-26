using System.Collections.Generic;

internal abstract class Skill
{
    public bool EnableIndicator { get; protected set; }
    private SkillTable.Row _skillTable;
    private readonly List<Effect> _effects = new();
    
    public void Initialize(SkillTable.Row skillTable)
    {
        _skillTable = skillTable;
        
        foreach (var effectId in skillTable.EffectIds)
        {
            var effectTable = TableManager.GetTable<EffectTable>().GetRow(effectId);
            var effect = EffectFactory.GetEffect(effectTable.EffectType);
            effect.Initialize(effectTable);
            _effects.Add(effect);
        }
    }
    
    public virtual void OnUpdate() { }
    public virtual void ShowIndicator() { }
    public virtual void HideIndicator() { }
    public abstract void Apply(IReadOnlyUnit owner);
}