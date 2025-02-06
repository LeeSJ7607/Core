using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GUIUtil
{
    public static GUIStyle TextFieldStyle()
    {
        return new GUIStyle(GUI.skin.textField);
    }
    
    public static GUIStyle LabelStyle(TextAnchor textAnchor = TextAnchor.MiddleCenter)
    {
        return new GUIStyle(GUI.skin.label)
        {
            alignment = textAnchor,
            normal = GUI.skin.button.normal,
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
    
    public static void DrawPopup(string name, ref int idx, string[] options, float width, Action act = null)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(width));
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
    
    public static void Desc(string desc, Color color = default)
    {
        var labelStyle = LabelStyle(TextAnchor.MiddleLeft);
        labelStyle.normal.textColor = color != default ? color : Color.white;
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(desc, labelStyle);
        EditorGUILayout.EndHorizontal();
    }
    
    public static void Desc(string key, string value, float keyWidth, float valueWidth)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(key, LabelStyle(TextAnchor.MiddleLeft), GUILayout.Width(keyWidth));
        if (!string.IsNullOrEmpty(value))
        {
            EditorGUILayout.LabelField(value, LabelStyle(TextAnchor.MiddleRight), GUILayout.Width(valueWidth));
        }
        
        EditorGUILayout.EndHorizontal();
    }

    public static void Desc(string key, string value, float keyWidth, float valueWidth, object lValue, object rValue)
    {
        if (lValue.Equals(rValue))
        {
            Desc(key, value, keyWidth, valueWidth);
        }
        else
        {
            DescColor(key, value, keyWidth, valueWidth, Color.cyan);
        }
    }

    private static void DescColor(string key, string value, float keyWidth, float valueWidth, Color color)
    {
        var keyLabelStyle = LabelStyle(TextAnchor.MiddleLeft);
        keyLabelStyle.normal.textColor = color;
        
        var valueLabelStyle = LabelStyle(TextAnchor.MiddleRight);
        valueLabelStyle.normal.textColor = color;
        
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(key, keyLabelStyle, GUILayout.Width(keyWidth));
        if (!string.IsNullOrEmpty(value))
        {
            EditorGUILayout.LabelField(value, valueLabelStyle, GUILayout.Width(valueWidth));
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    public static void Btn(string name, Action act)
    {
        if (GUILayout.Button(name, ButtonStyle()))
        {
            act?.Invoke();
        }
    }
    
    public static void Btn(string name, float width, Action act)
    {
        if (GUILayout.Button(name, ButtonStyle(), GUILayout.Width(width)))
        {
            act();
        }
    }
    
    public static void BtnExpand(string name, float width, Action act)
    {
        if (GUILayout.Button(name, ButtonStyle(), GUILayout.Width(width), GUILayout.ExpandWidth(true)))
        {
            act();
        }
    }
    
    public static void Btn(string name, float width, float height, Action act)
    {
        if (GUILayout.Button(name, ButtonStyle(), GUILayout.Width(width), GUILayout.Height(height)))
        {
            act();
        }
    }
    
    public static void Btn(Texture tex, float width, float height, Action act)
    {
        var option = new List<GUILayoutOption>(2)
        {
            GUILayout.Width(width),
            GUILayout.Height(height)
        };
        
        if (GUILayout.Button(tex, ButtonStyle(), option.ToArray()))
        {
            act();
        }
    }
}