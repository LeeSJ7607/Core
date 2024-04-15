using UnityEngine;
using UnityEditor;

public sealed class AssetManagementTool_Preview : EditorWindow
{
    private const float _size = 400;
    private Texture2D _texture2D;
    
    public static void Open(Texture2D texture2D)
    {
        var tool = GetWindow<AssetManagementTool_Preview>("Preview");
        tool.minSize = tool.maxSize = new Vector2(_size, _size);
        tool._texture2D = texture2D;
    }
    
    private void OnGUI()
    {
        GUIUtil.Btn(_texture2D, _size, _size, Close);
    }
}