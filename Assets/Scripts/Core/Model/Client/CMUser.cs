internal sealed class CMUser : IClientModel
{
    public int UserId { get; private set; }
    
    public void Sync()
    {
        var smUser = ModelManager.Instance.Get<SMUser>();
        UserId = smUser.UserId;
    }
}