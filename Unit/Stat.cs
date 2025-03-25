using System.Collections.Generic;

public enum EStat
{
    Max_HP,
    HP,
    ATK,
    WALK_SPEED,
}

public interface IReadOnlyStat
{
    long this[EStat type] { get; }
}

public sealed class Stat : IReadOnlyStat
{
    private readonly Dictionary<EStat, long> _statMap = new();
    
    public long this[EStat type]
    {
        get => _statMap.GetValueOrDefault(type, 0);
        set => Sum(type, value);
    }

    private void Sum(EStat type, long value)
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
        Sum(EStat.ATK, unitTable.Atk);
        Sum(EStat.WALK_SPEED, unitTable.Walk_Speed);
    }
}