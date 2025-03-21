using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class HierarchyTreeWindow : EditorWindow
{
	private Type[] _compositeTypes, _conditionTypes, _actionTypes;
    private bool _isCompositeFoldOut, _isConditionFoldOut, _isActionFoldOut;
    private Vector2 _scrollPos;
    
    [MenuItem("Custom/Window/HierarchyTreeWindow")]
    private static void Open()
    {
        GetWindow<HierarchyTreeWindow>().Show();
    }

    private void OnEnable()
    {
        var assembly = typeof(HierarchyTree).Assembly.GetExportedTypes();
        _compositeTypes = assembly.Where(_ => _.IsSubclassOf(typeof(HTComposite))).ToArray();
        _conditionTypes = assembly.Where(_ => _.IsSubclassOf(typeof(HTCondition))).ToArray();
        _actionTypes = assembly.Where(_ => _.IsSubclassOf(typeof(HTAction))).ToArray();
    }

	private void OnGUI()
	{
		_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

		GUILayout.BeginHorizontal();
		DrawZone();
		DrawDistrict();
		GUILayout.EndHorizontal();
		
		DrawComposite();
		DrawAction();
		DrawCondition();
		
		EditorGUILayout.EndScrollView();
	}

	private void DrawZone()
	{
		var name = nameof(Zone);

		if (!GUILayout.Button(name))
		{
			return;
		}

		var objs = FindObjectsByType<Zone>(FindObjectsSortMode.None);
		if (objs.IsNullOrEmpty())
		{
			Selection.activeObject = new GameObject(name, typeof(Zone));
		}
		else
		{
			EditorUtility.DisplayDialog("Error", $"{name}은 1개만 존재할 수 있습니다.", "OK");
		}
	}

	private void DrawDistrict()
	{
		var name = nameof(District);

		if (!GUILayout.Button(name))
		{
			return;
		}

		var objs = FindObjectsByType<District>(FindObjectsSortMode.None);
		var id = objs.IsNullOrEmpty() ? 0 : objs.Length;
		var obj = new GameObject($"{name}_{id}", typeof(District));
		obj.SetParent(GetOrCreateZone());
		Selection.activeObject = obj;

		GameObject GetOrCreateZone()
		{
			var zones = FindObjectsByType<Zone>(FindObjectsSortMode.None);
			return zones.IsNullOrEmpty() 
				? new GameObject(nameof(Zone), typeof(Zone)) 
				: zones[0].gameObject;
		}
	}

	private void DrawComposite()
	{
		_isCompositeFoldOut = EditorGUILayout.Foldout(_isCompositeFoldOut, $"{nameof(HTComposite)}");
		
		if (!_isCompositeFoldOut)
		{
			return;
		}
		
		foreach (var compositeType in _compositeTypes)
		{
			if (!GUILayout.Button(compositeType.Name))
			{
				continue;
			}
			
			var selectedObj = Selection.activeObject as GameObject;
			if (selectedObj == null)
			{
				EditorUtility.DisplayDialog("Error", $"{nameof(District)} 게임 오브젝트를 선택해주세요.", "OK");
				return;
			}
			
			if (selectedObj.TryGetComponent<District>(out var district))
			{
				CreateObj(compositeType, district.transform);
				return;
			}

			if (TryGetRoot<HTComposite>(out var composite))
			{
				CreateObj(compositeType, composite.transform);
				return;
			}
			
			EditorUtility.DisplayDialog("Error", $"{nameof(District)} 게임 오브젝트를 선택해주세요.", "OK");
		}

		void CreateObj(Type type, Transform root)
		{
			var obj = new GameObject(type.Name);
			obj.AddComponent(type);
			obj.SetParent(root);
			Selection.activeObject = obj;
		}
	}

	private void DrawAction()
	{
		_isActionFoldOut = EditorGUILayout.Foldout(_isActionFoldOut, $"{nameof(HTAction)}");

		if (!_isActionFoldOut)
		{
			return;
		}

		foreach (var actionType in _actionTypes)
		{
			if (!GUILayout.Button(actionType.Name))
			{
				continue;
			}

			if (TryGetRoot<HTComposite>(out var composite))
			{
				var obj = new GameObject(actionType.Name);
				obj.AddComponent(actionType);
				obj.SetParent(composite.gameObject);
				Selection.activeObject = obj;
			}
			else
			{
				EditorUtility.DisplayDialog("Error", $"{nameof(HTComposite)} 게임 오브젝트를 선택해주세요.", "OK");
			}
		}
	}

	private void DrawCondition()
	{
		_isConditionFoldOut = EditorGUILayout.Foldout(_isConditionFoldOut, $"{nameof(HTCondition)}");

		if (!_isConditionFoldOut)
		{
			return;
		}

		foreach (var conditionType in _conditionTypes)
		{
			if (!GUILayout.Button(conditionType.Name))
			{
				continue;
			}

			if (TryGetRoot<HTComposite>(out var composite))
			{
				var obj = new GameObject(conditionType.Name);
				obj.AddComponent(conditionType);
				obj.SetParent(composite.gameObject);
				Selection.activeObject = obj;
			}
			else
			{
				EditorUtility.DisplayDialog("Error", $"{nameof(HTComposite)} 게임 오브젝트를 선택해주세요.", "OK");
			}
		}
	}
	
	private bool TryGetRoot<T>(out T root)
	{
		root = default;
			
		var selectedObj = Selection.activeObject as GameObject;
		if (selectedObj == null)
		{
			return false;
		}

		var tm = selectedObj.transform;
		while (tm != null)
		{
			if (tm.TryGetComponent<T>(out var component))
			{
				root = component;
				return true;
			}

			tm = tm.parent;
		}

		return false;
	}
}