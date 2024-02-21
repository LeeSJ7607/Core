public sealed class AssetImporterConsts
{
    public enum AssetKind
    {
        Texture,
        FBX,
        Sound,
        End,
    }
    
    public enum FilterTexture
    {
        None,
        MipMap,
        References,
        Compare,
    }
    
    public enum SortTexture
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
    
    public enum SortFBX
    {
        Name,
        FileSize,
        ReadWrite,
        References,
    }
    
    public enum SortSound
    {
        Name,
        FileSize,
        ForceToMono,
        PreloadAudioData,
        CompressionFormat,
        LoadType,
        References,
    }
}