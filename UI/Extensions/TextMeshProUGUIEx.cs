using R3;
using TMPro;

internal sealed class TextMeshProUGUIEx : TextMeshProUGUI
{
    private CMOption _cmOption;
    private readonly CompositeDisposable _disposable = new();
    
    protected override void OnDestroy()
    {
        _disposable.Dispose();
        base.OnDestroy();
    }

    protected override void OnDisable()
    {
        _disposable.Clear();
        base.OnDisable();
    }
    
    public override string text
    {
        get => base.text;
        set => base.text = value;
    }

    public void SetLocalizedText(int localizeId)
    {
        if (localizeId == 0)
        {
            text = $"localizeId is 0";
            return;
        }

        _cmOption ??= ModelManager.Instance.Get<CMOption>();
        _cmOption.LanguageChanged.Subscribe(_ =>
        {
            //TODO: 로컬라이징.
            //text = localizeId;
        }).AddTo(_disposable);
    }
}