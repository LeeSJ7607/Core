using UnityEngine;
using UnityEditor;

public sealed class AssetImporterTool_Preview : EditorWindow
{
    private const float _size = 400;
    private Texture2D _texture2D;
    
    public static void Open(Texture2D texture2D)
    {
        var tool = GetWindow<AssetImporterTool_Preview>("Preview");
        tool.minSize = tool.maxSize = new Vector2(_size, _size);
        tool._texture2D = texture2D;
    }
    
    private void OnGUI()
    {
        if (GUILayout.Button(_texture2D, GUIUtil.ButtonStyle(), GUILayout.Width(_size), GUILayout.Height(_size)))
        {
            Close();
        }
    }
}