internal sealed class SkillController
{
    private readonly IReadOnlyUnit _unit;
    
    public SkillController(IReadOnlyUnit unit)
    {
        _unit = unit;
    }
}