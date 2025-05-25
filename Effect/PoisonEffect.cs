internal sealed class PoisonEffect : Effect
{
    public override void Apply(IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        throw new System.NotImplementedException();
    }
}