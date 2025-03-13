using UnityEngine;

public sealed class UIDamageText : MonoBehaviour, IObjectPool
{
    public float LifeTime => 3f;
    [SerializeField] private TextMeshProUGUIEx _txtDamage;

    public void Set(int damage)
    {
        _txtDamage.text = damage.ToStringMoney();
    }
}