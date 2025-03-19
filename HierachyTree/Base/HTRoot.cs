using UnityEngine;

public sealed class HTRoot : MonoBehaviour
{
    private IHTComposite _htComposite;

    private void Awake()
    {
        _htComposite = GetComponentInChildren<IHTComposite>();
        if (_htComposite == null)
        {
            Debug.LogError($"{nameof(HTRoot)} 자식 오브젝트에 {nameof(HTComposite)}가 컴포넌트 되어있지 않습니다.");
        }
    }

    private void Update()
    {
        if (_htComposite == null)
        {
            return;
        }

        if (_htComposite.Update() == EBTStatus.Running)
        {
            return;
        }
        
        gameObject.Hide();
    }
}