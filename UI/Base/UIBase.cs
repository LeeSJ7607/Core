using R3;
using UnityEngine;

internal interface IReadOnlyUIBase
{
    void SetSlotCreator(ISlotCreator slotCreator);
}

public abstract class UIBase : MonoBehaviour, IReadOnlyUIBase, IMVCView
{
    private IMVCController _mvcController;
    protected ISlotCreator _slotCreator;
    protected readonly CompositeDisposable _disposable = new();
    
    void IReadOnlyUIBase.SetSlotCreator(ISlotCreator slotCreator)
    {
        _slotCreator = slotCreator;
    }
    
    protected void SetMVCController(IMVCController mvcController)
    {
        _mvcController = mvcController;
        _mvcController.Initialize(this);
    }
    
    protected virtual void OnDestroy()
    {
        _disposable.Dispose();
        _mvcController?.Release();
        _mvcController = null;
    }
    
    protected virtual void Awake()
    {
        
    }
    
    public virtual void Show()
    {
        gameObject.Show();
    }
    
    public virtual void Hide()
    {
        _disposable.Clear();
        gameObject.Hide();
    }
}