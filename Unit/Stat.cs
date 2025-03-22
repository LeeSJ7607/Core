using System.Collections.Generic;

public enum EStat
{
    Max_HP,
    HP,
}

public sealed class Stat
{
    private readonly Dictionary<EStat, int> _statMap = new();
    
    public int this[EStat type]
    {
        get => _statMap.GetValueOrDefault(type, 0);
        set => Sum(type, value);
    }

    private void Sum(EStat type, int value)
    {
        if (!_statMap.TryAdd(type, value))
        {
            _statMap[type] = value;
        }
    }

    public void Initialize(IReadOnlyUnit owner)
    {
        var unitTable = owner.UnitTable;
        Sum(EStat.Max_HP, unitTable.Max_HP);
        Sum(EStat.HP, unitTable.Max_HP);
    }
}