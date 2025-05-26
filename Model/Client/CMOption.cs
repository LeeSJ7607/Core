using R3;

internal sealed class CMOption : Model
{
    public ReadOnlyReactiveProperty<R3.Unit> LanguageChanged => _languageChanged;
    private readonly ReactiveProperty<R3.Unit> _languageChanged = new();
    
    protected override void OnRelease()
    {
        _languageChanged.Dispose();
    }
}