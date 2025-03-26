using UnityEngine;

[DisallowMultipleComponent] [RequireComponent(typeof(HTSequence))]
public sealed class District : MonoBehaviour
{
    public bool IsCleared { get; set; }
    private bool _isActive;
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
            initializer.Initialize(battleEnvironment.UnitController);
        }

        _isActive = true;
    }

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }

        if (_htComposite.Update() == EBTStatus.Running)
        {
            return;
        }

        _htComposite.Release();
    }
}