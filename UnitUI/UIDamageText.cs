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
    }
}