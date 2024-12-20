using TMPro;
using UnityEngine.UI;

internal sealed class SampleView : UIPopup
{
    private TextMeshProUGUI _pointTxt;
    private Button _addPointBtn;
    private SampleController _sampleController;

    protected override void OnDestroy()
    {
        _addPointBtn.RemoveClick();
        base.OnDestroy();
    }

    protected override void Awake()
    {
        base.Awake();
        CreateMVCController(_sampleController);
        _addPointBtn.AddClick(OnClick_AddPoint);
    }

    public void SetPoint(int point)
    {
        _pointTxt.text = point.ToString();
    }

    private void OnClick_AddPoint()
    {
        _sampleController.SetRankingPoint(10);
    }
}