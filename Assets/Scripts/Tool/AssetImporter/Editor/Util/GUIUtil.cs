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
    
    public static void Desc(string desc)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(desc, LabelStyle(TextAnchor.MiddleLeft));
        EditorGUILayout.EndHorizontal();
    }
    
    public static void Desc(string key, string value, float keyWidth, float valueWidth)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(key, LabelStyle(TextAnchor.MiddleLeft), GUILayout.Width(keyWidth));
        if (string.IsNullOrEmpty(value) == false)
        {
            EditorGUILayout.LabelField(value, LabelStyle(TextAnchor.MiddleRight), GUILayout.Width(valueWidth));
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    public static void DescColor(string key, string value, float keyWidth, float valueWidth)
    {
        var color = Color.red;
        
        var keyLabelStyle = LabelStyle(TextAnchor.MiddleLeft);
        keyLabelStyle.normal.textColor = color;
        
        var valueLabelStyle = LabelStyle(TextAnchor.MiddleRight);
        valueLabelStyle.normal.textColor = color;
        
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(key, keyLabelStyle, GUILayout.Width(keyWidth));
        if (string.IsNullOrEmpty(value) == false)
        {
            EditorGUILayout.LabelField(value, valueLabelStyle, GUILayout.Width(valueWidth));
        }
        
        EditorGUILayout.EndHorizontal();
    }
}