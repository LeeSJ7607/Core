using System.Collections.Generic;

public enum eStat
{
    Max_HP,
    HP,
    ATK,
    WALK_SPEED,
}

public interface IReadOnlyStat
{
    long this[eStat type] { get; }
}

public sealed class Stat : IReadOnlyStat
{
    private readonly Dictionary<eStat, long> _statMap = new();
    
    public long this[eStat type]
    {
        get => _statMap.GetValueOrDefault(type, 0);
        set => Sum(type, value);
    }

    private void Sum(eStat type, long value)
    {
        if (!_statMap.TryAdd(type, value))
        {
            _statMap[type] = value;
        }
    }

    public void Initialize(IReadOnlyUnit owner)
    {
        var unitTable = owner.UnitTable;
        Sum(eStat.Max_HP, unitTable.Max_HP);
        Sum(eStat.HP, unitTable.Max_HP);
        Sum(eStat.ATK, unitTable.Atk);
        Sum(eStat.WALK_SPEED, unitTable.Walk_Speed);
    }
}