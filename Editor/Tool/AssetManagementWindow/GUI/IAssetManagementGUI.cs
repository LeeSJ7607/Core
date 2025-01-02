using UnityEngine;

public interface IAssetManagementGUI
{
    public int Order { get; }
    public int TotalCnt { get; }
    public Vector2 ScrollPos { get; set; }
    public IAssetManagementImpl OriginAssetManagementImpl { get; }
    public IAssetManagementImpl AssetManagementImpl { get; }
    public void Initialize(string selectedFilePath);
    public void Draw();
    public bool CanDiff();
    public bool TrySave();
}