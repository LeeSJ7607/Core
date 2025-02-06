using UnityEditor;
using UnityEngine;

//TODO: 좀 더 이쁘게.
[CustomEditor(typeof(Localize))]
public sealed class EditorLocalize : Editor
{
    private Vector2 _scrollPos;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var localize = (Localize)target;
        if (localize == null || localize.TryGetText(out var localizeTableMap_, out _) == false)
        {
            return;
        }

        var keyLabel = new GUIStyle(GUI.skin.label);
        keyLabel.normal.textColor = Color.yellow;

        var valueLebel = new GUIStyle(GUI.skin.label);
        valueLebel.normal.textColor = Color.cyan;

        foreach (var pair in localizeTableMap_)
        {
            var systemLanguage = pair.Key;
            var text = pair.Value;

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(systemLanguage.ToString(), keyLabel);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                var str = text.Split('\n');
                var height = str.IsNullOrEmpty() ? 0 : str.Length * 20;
                
                _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(height));
                GUILayout.Label(text, valueLebel);
                GUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
        }
    }
}