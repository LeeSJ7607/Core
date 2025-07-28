internal sealed class PosionBuff : Buff
{
    public override void OnUpdate()
    {
        base.OnUpdate();
        ApplyPoisonDamage();
    }

    private void ApplyPoisonDamage()
    {
        _target.Hit(CalcValue());
    }
}