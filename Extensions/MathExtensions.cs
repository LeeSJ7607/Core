public static class MathExtensions
{
    public static float ToRatio(this int percentage)
    {
        return (float)percentage / GameRuleConst.PERCENTAGE_BASE;
    }

    public static float ToRatio(this float percentage)
    {
        return percentage / GameRuleConst.PERCENTAGE_BASE;
    }
}