internal sealed class Receiver
{
    private readonly Session _session;

    public Receiver(Session session)
    {
        _session = session;
    }

    public void ReceiveProcess(Sender.Request request, EResponseResult responseResult)
    {
        switch (responseResult)
        {
        case EResponseResult.Success:
            {
                var responseType = request.ResponseType;
                var buffer = request.WebRequest.downloadHandler.data;
                var body = _session.Serializer.Deserialize(responseType, buffer);

                NetworkManager.Instance.Receive(responseType, body);
            }
            break;

        case EResponseResult.Shutdown:
            {
                //TODO: 네트워크 연결이 끊겼다는 팝업을 보여줘야함.
            }
            break;
        }
    }
}