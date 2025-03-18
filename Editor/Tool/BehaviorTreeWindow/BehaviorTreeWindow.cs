using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal sealed class BehaviorTreeWindow : EditorWindow
{
    private readonly BehaviorTree[] _nodes;
    
    public BehaviorTreeWindow()
    {
        _nodes = typeof(BehaviorTree).Assembly.GetExportedTypes()
                               .Where(_ => _.IsInterface == false && _.IsAbstract == false)
                               .Where(_ => typeof(BehaviorTree).IsAssignableFrom(_))
                               .Select(_ => (BehaviorTree)Activator.CreateInstance(_))
                               .ToArray();
    }
    
    [MenuItem("Custom/Window/BehaviorTreeWindow")]
    public static void Open()
    {
        GetWindow<BehaviorTreeWindow>();
    }

    private void OnGUI()
    {
        foreach (var node in _nodes)
        {
            if (!GUILayout.Button(node.ToString(), GUIUtil.ButtonStyle(), GUILayout.Height(30)))
            {
                return;
            }
        }
    }
}