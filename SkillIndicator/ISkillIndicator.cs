internal interface ISkillIndicator
{
    void Initialize(IReadOnlyUnit owner, SkillTable.Row skillTable);
    void OnUpdate();
    void Show();
    void Hide();
}