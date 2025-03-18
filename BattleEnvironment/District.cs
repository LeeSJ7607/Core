using UnityEngine;

public interface IDistrict
{
    bool IsActive { get; set; }
    bool IsCleared { get; }
}

public sealed class District : MonoBehaviour, IDistrict
{
    bool IDistrict.IsActive { get; set; }
    bool IDistrict.IsCleared { get; }
}