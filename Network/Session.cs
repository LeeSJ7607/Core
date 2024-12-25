internal sealed class Session
{
    private readonly Sender _sender;
    public Receiver Receiver { get; private set; }
    public Serializer Serializer { get; private set; }
    public bool IsValidSession { get; private set; }

    public Session()
    {
        _sender = new Sender(this);
        Receiver = new Receiver(this);
        Serializer = new Serializer();
        IsValidSession = true;
    }
    
    public void Release()
    {
        _sender.Release();
        IsValidSession = false;
    }

    public void Send<TResponse>(IRequest request) where TResponse : IResponse
    {
        if (IsValidSession)
        {
            _sender.Push<TResponse>(request);
        }
    }
}