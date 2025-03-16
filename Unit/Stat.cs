using System.Collections.Generic;

public enum EStat
{
    Max_HP,
    HP,
}

public sealed class Stat
{
    private readonly Dictionary<EStat, int> _statMap = new();
    
    public Stat(IReadOnlyUnit owner)
    {
        //TODO: 테이블 화.
        Sum(EStat.Max_HP, 1000);
        Sum(EStat.HP, 1000);
    }

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
}