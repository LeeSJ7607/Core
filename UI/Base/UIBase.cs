using UnityEngine;

public interface IReadOnlyUIBase
{
    void SetSlotCreator(ISlotCreator slotCreator);
}

public abstract class UIBase : MonoBehaviour, IReadOnlyUIBase, IMVCView
{
    protected ISlotCreator _slotCreator;
    private IMVCController _mvcController;
    
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
        gameObject.Hide();
    }
}