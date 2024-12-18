using UnityEngine;

internal abstract class UIBase : MonoBehaviour
{
    public virtual void Show() => gameObject.Show();
    public virtual void Hide() => gameObject.Hide();
}