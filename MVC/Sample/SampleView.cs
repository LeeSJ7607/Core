using TMPro;
using UnityEngine.UI;

internal sealed class SampleView : UIPopup<SampleController>
{
    private TextMeshProUGUI _pointTxt;
    private Button _addPointBtn;

    protected override void OnDestroy()
    {
        _addPointBtn.RemoveClick();
        base.OnDestroy();
    }

    protected override void Awake()
    {
        base.Awake();
        _addPointBtn.AddClick(OnClick_AddPoint);
    }

    public void SetPoint(int point)
    {
        _pointTxt.text = point.ToString();
    }

    private void OnClick_AddPoint()
    {
        _mvcController.SetRankingPoint(10);
    }
}