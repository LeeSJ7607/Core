using UnityEngine;

public sealed class AssetImporterGUI_Sound : IAssetImporterGUI
{
    public int Order => 2;
    public int TotalCnt { get; }
    public Vector2 ScrollPos { get; set; }
    public IAssetImporterImpl OriginAssetImporterImpl { get; }
    public IAssetImporterImpl AssetImporterImpl { get; }
    
    public void Initialize(string selectedFilePath)
    {
        
    }
    
    public void Draw()
    {
        
    }
    
    public bool CanDiff() => false;
    public bool TrySave() => true;
}