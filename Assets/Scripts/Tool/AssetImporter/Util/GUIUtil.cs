using System;
using UnityEditor;
using UnityEngine;

public sealed class GUIUtil
{
    public static GUIStyle LabelStyle(TextAnchor textAnchor = TextAnchor.MiddleCenter)
    {
        return new GUIStyle(GUI.skin.label)
        {
            alignment = textAnchor,
            normal = GUI.skin.button.normal
        };
    }
    
    public static GUIStyle ButtonStyle(TextAnchor textAnchor = TextAnchor.MiddleCenter)
    {
        return new GUIStyle(GUI.skin.button)
        {
            alignment = textAnchor,
            normal = GUI.skin.button.normal
        };
    }
    
    public static GUIStyle HelpBoxStyle(Texture2D tex)
    {
        if (tex == null)
        {
            return EditorStyles.helpBox;
        }

        return new GUIStyle(EditorStyles.helpBox)
        {
            normal =
            {
                background = tex
            }
        };
    }
    
    public static void DrawPopup(string name, ref int idx, string[] options, Action act = null)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUI.BeginChangeCheck();
            idx = EditorGUILayout.Popup(name, idx, options);

            if (EditorGUI.EndChangeCheck())
            {
                act?.Invoke();
            }
        }
        EditorGUILayout.EndVertical();
    }
}