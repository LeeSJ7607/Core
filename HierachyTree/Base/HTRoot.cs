using UnityEngine;

[DisallowMultipleComponent] [RequireComponent(typeof(HTSequence))]
public sealed class HTRoot : MonoBehaviour
{
    private IHTComposite _htComposite;

    private void Awake()
    {
        _htComposite = GetComponent<IHTComposite>();
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