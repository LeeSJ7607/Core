public interface IAttacker
{
    bool IsAttackable { get; set; }
    long Damage { get; }
}

public abstract partial class Unit
{
    bool IAttacker.IsAttackable { get; set; } = true;
    long IAttacker.Damage => _stat[EStat.ATK];
}