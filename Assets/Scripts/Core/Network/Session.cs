internal sealed class Session
{
    private readonly Sender _sender;
    public Receiver Receiver { get; private set; }
    public Serializer Serializer { get; private set; }

    public Session()
    {
        _sender = new Sender(this);
        Receiver = new Receiver(this);
        Serializer = new Serializer();
    }

    public void Send<TResponse>(IRequest request) where TResponse : IResponse
    {
        _sender.Push<TResponse>(request);
    }
}