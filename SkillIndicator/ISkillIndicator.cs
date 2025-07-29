using UnityEngine;

internal interface ISkillIndicator
{
    Vector3 Direction { get; }
    void Initialize(IReadOnlyUnit owner, SkillTable.Row skillTable);
    void OnUpdate();
    void Show();
    void Hide();
}