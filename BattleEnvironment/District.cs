using System;
using UnityEngine;

public interface IDistrict
{
    bool IsActive { get; set; }
}

public sealed class District : MonoBehaviour, IDistrict
{
    bool IDistrict.IsActive { get; set; }

    private void Awake()
    {
        
    }
}