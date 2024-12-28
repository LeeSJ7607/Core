internal sealed class GameManager : MonoSingleton<GameManager> 
{
    public void Release()
    {
        ModelManager.Instance.Release();
    }
    
    public void Initialize()
    {
        ModelManager.Instance.Initialize();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        ModelManager.Instance.OnUpdate();
    }

    private void OnApplicationQuit()
    {
        ModelManager.Instance.SaveAll();
    }
}