using MemoryPack;

public interface IRequest
{
    string Url { get; }
}

[MemoryPackable]
public sealed class LoginReq : IRequest
{
    [MemoryPackIgnore]
    string IRequest.Url => "Login";

    public string NickName { get; set; }
}