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
        _cmOption.LanguageChanged
                 .Subscribe(_ => text = LocalizeUtil.GetText(localizeId))
                 .AddTo(_disposable);
    }
}

internal static class LocalizeUtil
{
    public static string GetText(int localizeId)
    {
        //TODO: 스트링 테이블에서 값을 가져온 뒤 리턴
        //TODO: 스트링 테이블에 존재하지 않을 경우 localizeId 을 리턴.
        return $"localizeId is {localizeId}";
    }
}