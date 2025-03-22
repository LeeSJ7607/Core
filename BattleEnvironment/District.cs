using UnityEngine;

[DisallowMultipleComponent] [RequireComponent(typeof(HTSequence))]
public sealed class District : MonoBehaviour
{
    public bool IsCleared { get; set; }
    private IHTComposite _htComposite;

    private void Awake()
    {
        _htComposite = GetComponent<IHTComposite>();   
    }

    public void Initialize(IReadOnlyBattleEnvironment battleEnvironment)
    {
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