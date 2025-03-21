using UnityEngine;

[DisallowMultipleComponent] [RequireComponent(typeof(HTRoot))]
public sealed class District : MonoBehaviour
{
    public bool IsCleared { get; set; }
}