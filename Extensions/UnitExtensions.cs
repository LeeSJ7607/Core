public static class UnitExtensions
{
    public static bool IsDead(this Unit unit)
    {
        return unit == null || unit.IsDead;
    }
}