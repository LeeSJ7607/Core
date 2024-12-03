public sealed class AssetManagementConsts
{
    public enum EAssetKind
    {
        Texture,
        FBX,
        Sound,
        End,
    }
    
    public enum EFilterTexture
    {
        None,
        MipMap,
        References,
        Compare,
    }
    
    public enum ESortTexture
    {
        Name,
        FileSize,
        TextureSize,
        MipMap,
        Format,
        WrapMode,
        FilterMode,
        TextureType,
        References,
        Compare,
    }
    
    public enum ESortFBX
    {
        Name,
        FileSize,
        ReadWrite,
        References,
    }
    
    public enum ESortSound
    {
        Name,
        FileSize,
        ForceToMono,
        PreloadAudioData,
        CompressionFormat,
        SampleRateSetting,
        LoadType,
        References,
    }
}