using System;
using System.Linq;

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

public enum FilterTexture
{
    None,
    MipMap,
    References,
    Compare,
}

public abstract class AssetImporterPart
{
    protected int _selectedTextureSortIdx;
    protected readonly string[] _sortTextures = Enum.GetNames(typeof(SortTexture)).ToArray();
    
    protected int _selectedTextureFilterIdx;
    protected readonly string[] _filterTextures = Enum.GetNames(typeof(FilterTexture)).ToArray();
    
    public abstract string Name { get; }
    public abstract int TextureCnt { get; }
    public virtual int FBXCnt { get; }
    public abstract bool IsOn { get; set; }
    public abstract void Draw();
    public abstract void ShowDiff();
    public abstract bool TrySave();
}