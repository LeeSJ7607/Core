using UnityEngine;

public interface IAssetImporterGUI
{
    public int Order { get; }
    public int TotalCnt { get; }
    public Vector2 ScrollPos { get; set; }
    public IAssetImporterImpl OriginAssetImporterImpl { get; }
    public IAssetImporterImpl AssetImporterImpl { get; }
    public void Initialize(string selectedFilePath);
    public void Draw();
    public bool CanDiff();
    public bool TrySave();
}