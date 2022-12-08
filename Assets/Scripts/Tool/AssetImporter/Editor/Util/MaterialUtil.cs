using System.Collections.Generic;
using UnityEngine;

public static class MaterialUtil
{
    public static IEnumerable<Texture> GetTextures(Material mat)
    {
        var result = new List<Texture>(10);
        var propertyNames = mat.GetTexturePropertyNames();
                
        foreach (var propertyName in propertyNames)
        {
            var tex = mat.GetTexture(propertyName);
            if (tex != null)
            {
                result.Add(tex);
            }
        }

        return result;
    }
}