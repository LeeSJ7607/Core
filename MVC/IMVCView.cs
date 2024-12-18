internal interface IMVCView
{
    //TODO: 뷰는 모델을 몰라야하는데 모델을 꼭 넘겨줘야할까??
    void Bind(IMVCModel model);
}