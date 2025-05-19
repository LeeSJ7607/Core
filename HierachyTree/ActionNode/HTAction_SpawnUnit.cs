using UnityEngine;

[DisallowMultipleComponent]
public sealed class HTAction_SpawnUnit : HTAction
{
    private ISpawner[] _spawners;

    public override void OnBegin()
    {
        base.OnBegin();
        _spawners = GetComponents<ISpawner>();

        if (_spawners.IsNullOrEmpty())
        {
            Debug.LogError($"{nameof(HTAction_SpawnUnit)}에 컴포넌트된 {nameof(ISpawner)}이 없습니다.");
        }
    }

    public override eBTStatus OnUpdate()
    {
        _spawners.Foreach(_ => _.Spawn());
        return eBTStatus.Success;
    }
}