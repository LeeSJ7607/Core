using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

internal abstract class BaseCanvas : MonoBehaviour
{
    private readonly UIContainer _uiContainer = new();
    private readonly Stack<UIPopup> _popups = new();
    
    protected virtual void Awake()
    {
        CreateCanvas();
        CreateCanvasScale();
        gameObject.AddComponent<GraphicRaycaster>();
    }
    
    private void CreateCanvas()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 
          | AdditionalCanvasShaderChannels.Normal 
          | AdditionalCanvasShaderChannels.Tangent;
    }
    
    private void CreateCanvasScale()
    {
        var canvasScale = gameObject.AddComponent<CanvasScaler>();
        canvasScale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScale.referenceResolution = new Vector2(1920, 1080); //TODO: Const Table. (스크립터블 오브젝트)
        canvasScale.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScale.matchWidthOrHeight = 1f;
    }
    
    public void OnTick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PopPopup();
        }
    }
    
    private void PopPopup()
    {
        if (_popups.IsNullOrEmpty())
        {
            //ShowPopup<UIPopup_System>();
            return;
        }

        var popup = _popups.Pop();
        popup.Hide();
    }
    
    public T ShowPopup<T>() where T : UIPopup
    {
        var popup = _uiContainer.GetOrCreate<T>(transform);
        popup.Show();
        
        _popups.Push(popup);
        return popup;
    }
}