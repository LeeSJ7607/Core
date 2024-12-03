using MemoryPack;

public interface IRequest
{
    string Url { get; }
}

[MemoryPackable]
public partial class LoginReq : IRequest
{
    [MemoryPackIgnore]
    string IRequest.Url => "Login";

    public string NickName { get; set; }
}