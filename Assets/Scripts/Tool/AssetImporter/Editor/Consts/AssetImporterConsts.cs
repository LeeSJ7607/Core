public sealed class AssetImporterConsts
{
    public enum AssetType
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
        ReadAndWrite,
        References,
    }
}