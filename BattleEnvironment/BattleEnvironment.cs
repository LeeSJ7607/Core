public interface IReadOnlyBattleEnvironment
{
    IUnitController UnitController { get; }
}

public sealed class BattleEnvironment : IReadOnlyBattleEnvironment
{
    IUnitController IReadOnlyBattleEnvironment.UnitController { get; } = new UnitController();
}