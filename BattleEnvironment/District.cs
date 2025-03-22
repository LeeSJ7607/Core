using UnityEngine;

[DisallowMultipleComponent] [RequireComponent(typeof(HTRoot))]
public sealed class District : MonoBehaviour
{
    public bool IsCleared { get; set; }
    private HTRoot _htRoot;

    private void Awake()
    {
        _htRoot = GetComponent<HTRoot>();
    }

    public void Initialize(IReadOnlyBattleEnvironment battleEnvironment)
    {
        _htRoot.Initialize(battleEnvironment);
    }
}