using System;

internal sealed class CMUser : Model
{
    public Guid UserId { get; private set; } = Guid.NewGuid();

    protected override void OnRelease()
    {
        
    }

    public void Set(SMUser smUser)
    {
        UserId = smUser.UserId;
    }
}