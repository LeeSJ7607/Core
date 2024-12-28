internal sealed class Receiver
{
    private readonly Session _session;

    public Receiver(Session session)
    {
        _session = session;
    }

    public void ReceiveProcess(EResponseResult responseResult, Sender.Request request)
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
                _session.Release();
                
                var popup = UIManager.Instance.ShowPopup<UIPopup_System>();
                popup.Set("서버가 종료되었습니다.");
            }
            break;
        }
    }
}