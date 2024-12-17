using UnityEngine;

public class UIBase : MonoBehaviour
{
    public virtual void Show() => gameObject.Show();
    public virtual void Hide() => gameObject.Hide();
}