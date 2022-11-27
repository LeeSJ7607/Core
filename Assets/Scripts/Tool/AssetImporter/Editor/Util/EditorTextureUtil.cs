using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// <see cref="UnityEditor.TextureUtil"/> Accessor
/// </summary>
/// <author>Seibe TAKAHASHI</author>
/// <remarks>
/// (c) 2017 Seibe TAKAHASHI.
/// This code is released under the MIT License.
/// http://opensource.org/licenses/mit-license.php
/// </remarks>
public static class EditorTextureUtil
{
    private static readonly System.Type cType;
    private static MethodInfo mMethod_GetMipmapCount;
    private static MethodInfo mMethod_GetTextureFormat;
    private static MethodInfo mMethod_GetRuntimeMemorySizeLong;
    private static MethodInfo mMethod_GetStorageMemorySizeLong;
    private static MethodInfo mMethod_IsNonPowerOfTwo;

    static EditorTextureUtil()
    {
        cType = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
        Assert.IsNotNull(cType);
    }

    public static int GetMipmapCount(Texture texture)
    {
        if (mMethod_GetMipmapCount == null)
            mMethod_GetMipmapCount = cType.GetMethod("GetMipmapCount", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(mMethod_GetMipmapCount);
        return (int)mMethod_GetMipmapCount.Invoke(null, new[] { texture });
    }

    public static TextureFormat GetTextureFormat(Texture texture)
    {
        if (mMethod_GetTextureFormat == null)
            mMethod_GetTextureFormat = cType.GetMethod("GetTextureFormat", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(mMethod_GetTextureFormat);
        return (TextureFormat)mMethod_GetTextureFormat.Invoke(null, new[] { texture });
    }

    public static long GetRuntimeMemorySize(Texture texture)
    {
        if (mMethod_GetRuntimeMemorySizeLong == null)
            mMethod_GetRuntimeMemorySizeLong = cType.GetMethod("GetRuntimeMemorySizeLong", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(mMethod_GetRuntimeMemorySizeLong);
        return (long)mMethod_GetRuntimeMemorySizeLong.Invoke(null, new[] { texture });
    }

    public static long GetStorageMemorySize(Texture texture)
    {
        if (mMethod_GetStorageMemorySizeLong == null)
            mMethod_GetStorageMemorySizeLong = cType.GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(mMethod_GetStorageMemorySizeLong);
        return (long)mMethod_GetStorageMemorySizeLong.Invoke(null, new[] { texture });
    }

    public static bool IsNonPowerOfTwo(Texture2D texture)
    {
        if (mMethod_IsNonPowerOfTwo == null)
            mMethod_IsNonPowerOfTwo = cType.GetMethod("IsNonPowerOfTwo", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(mMethod_IsNonPowerOfTwo);
        return (bool)mMethod_IsNonPowerOfTwo.Invoke(null, new[] { texture });
    }
    
    public static string TextureSize(Texture2D tex)
    {
        var size = GetStorageMemorySize(tex);
        return EditorUtility.FormatBytes(size);
    }
    
    public static TextureFormat GetTextureFormat(TextureImporterFormat type)
    {
        return type switch
        {
            TextureImporterFormat.ASTC_12x12 => TextureFormat.ASTC_12x12,
            TextureImporterFormat.ASTC_10x10 => TextureFormat.ASTC_10x10,
            TextureImporterFormat.ASTC_8x8 => TextureFormat.ASTC_8x8,
            TextureImporterFormat.ASTC_6x6 => TextureFormat.ASTC_6x6,
            TextureImporterFormat.ASTC_5x5 => TextureFormat.ASTC_5x5,
            TextureImporterFormat.ASTC_4x4 => TextureFormat.ASTC_4x4,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    public static void ChangeReadable(TextureImporter importer, bool active)
    {
        if (importer.isReadable == active)
        {
            return;
        }

        importer.isReadable = active;
        importer.SaveAndReimport();
    }

    public static bool IsSameTexture(Texture2D left, Texture2D right)
    {
        var leftPixels = left.GetPixels();
        var rightPixels = right.GetPixels();

        if (leftPixels.Length != rightPixels.Length)
        {
            return false;
        }

        for (var i = 0; i < leftPixels.Length; i++)
        {
            if (leftPixels[i] != rightPixels[i])
            {
                return false;
            }
        }

        return true;
    }
}