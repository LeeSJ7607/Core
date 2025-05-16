public sealed class SkillController
{
    private BaseSkill[] _skills;
    
    public void Initialize(IReadOnlyUnit owner)
    {
        var cnt = EnumUtil.GetLength<eSkillSlot>();
        _skills = new BaseSkill[cnt];
    }
}