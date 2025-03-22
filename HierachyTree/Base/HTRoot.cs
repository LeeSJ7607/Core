using UnityEngine;

//TODO: 리콰이어컴포넌트 필요?
[DisallowMultipleComponent] [RequireComponent(typeof(HTSequence))]
public sealed class HTRoot : MonoBehaviour
{
    private IHTComposite _htComposite;

    public void Initialize(IReadOnlyBattleEnvironment battleEnvironment)
    {
        _htComposite = GetComponent<IHTComposite>();
        var initializers = GetComponentsInChildren<IUnitControllerBinder>();
        
        foreach (var initializer in initializers)
        {
            initializer.Initialize((IUnitController)battleEnvironment);
        }
    }

    private void Update()
    {
        if (_htComposite != null)
        {
            _htComposite.Update();
        }
    }
}