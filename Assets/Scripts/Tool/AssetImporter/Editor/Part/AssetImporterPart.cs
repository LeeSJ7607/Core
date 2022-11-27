public abstract class AssetImporterPart
{
    public abstract string Name { get; }
    public abstract bool IsOn { get; set; }
    public abstract void Draw();
    public abstract void EndCompare();
    public abstract bool TrySave();
}