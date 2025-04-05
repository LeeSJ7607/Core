using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Billboard))]
public sealed class UIDamageText : MonoBehaviour, IObjectPool
{
    public float LifeTime => 3f;
    private TextMeshProUGUIEx _txtDamage;
    
    private void Awake()
    {
        _txtDamage = GetComponent<TextMeshProUGUIEx>();
    }

    public void Set(long damage)
    {
        _txtDamage.text = damage.ToStringMoney();

        _txtDamage.alpha = 1f;
        _txtDamage.DOFade(0f, LifeTime)
                  .SetEase(Ease.Linear);
        
        transform.localPosition = Vector3.zero;
        transform.DOMoveY(transform.position.y + 2f, LifeTime)
                 .SetEase(Ease.Linear);
    }
}