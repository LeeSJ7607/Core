public interface IReadOnlyBattleEnvironment
{
    UnitContainer UnitContainer { get; }
}

public sealed class BattleEnvironment : IReadOnlyBattleEnvironment
{
    UnitContainer IReadOnlyBattleEnvironment.UnitContainer { get; } = new();
}