using UnityEngine;

[DisallowMultipleComponent]
public sealed class HTCondition_ClearDistrictAll : HTCondition
{
    private District[] _districts;

    public override void OnBegin()
    {
        base.OnBegin();
        _districts = FindObjectsByType<District>(FindObjectsSortMode.None);
        
        if (_districts.IsNullOrEmpty())
        {
            Debug.LogError($"{nameof(HTCondition_ClearDistrictAll)}에 컴포넌트된 {nameof(District)}이 없습니다.");
        }
    }

    public override EBTStatus OnUpdate()
    {
        foreach (var district in _districts)
        {
            if (!district.IsCleared)
            {
                return EBTStatus.Success;
            }
        }

        return EBTStatus.Success;
    }
}