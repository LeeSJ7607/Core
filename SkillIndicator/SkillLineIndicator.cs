using UnityEngine;
using UnityEngine.InputSystem;

internal sealed class SkillLineIndicator : ISkillIndicator
{
    Vector3 ISkillIndicator.Direction => _direction;
    private Vector3 _direction;
    private Transform _ownerTm;
    private float _skillRange;
    private Camera _mainCam;
    private LineRenderer _lineRenderer;
    
    void ISkillIndicator.Initialize(IReadOnlyUnit owner, SkillTable.Row skillTable)
    {
        _ownerTm = owner.Tm;
        _skillRange = skillTable.SkillRange;
        _mainCam = Camera.main;
    }
    
    void ISkillIndicator.OnUpdate()
    {
        if (!CanUpdateLineRenderer())
        {
            return;
        }
        
        if (!TryGetMouseDirection(out var mouseDirection))
        {
            return;
        }
        
        UpdateLineRenderer(mouseDirection);
        _direction = mouseDirection;
    }
    
    void ISkillIndicator.Show()
    {
        CreateLineRenderer();
        _lineRenderer.enabled = true;
    }
    
    void ISkillIndicator.Hide()
    {
        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = false;
        }
    }

    private void CreateLineRenderer()
    {
        if (_lineRenderer != null)
        {
            return;
        }

        var obj = new GameObject(nameof(SkillLineIndicator));
        _lineRenderer = obj.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }

    private bool CanUpdateLineRenderer()
    {
        return _lineRenderer != null && _lineRenderer.enabled;
    }

    private bool TryGetMouseDirection(out Vector3 mouseDirection)
    {
        if (!TryGetMousePosition(out var mousePos))
        {
            mouseDirection = Vector3.zero;
            return false;
        }

        var start = _ownerTm.position;
        var direction = (mousePos - start);
        direction.y = 0f;
        
        if (direction.sqrMagnitude < 0.01f)
        {
            mouseDirection = Vector3.zero;
            return false;
        }
        
        mouseDirection = direction.normalized;
        return true;
    }

    private bool TryGetMousePosition(out Vector3 mousePos)
    {
        var readValue = Mouse.current.position.ReadValue();
        var ray = _mainCam.ScreenPointToRay(readValue);
        
        if (!Physics.Raycast(ray, out var hit))
        {
            mousePos = Vector3.zero;
            return false;
        }
        
        mousePos = hit.point;
        return true;
    }
    
    private void UpdateLineRenderer(Vector3 dir)
    {
        var start = _ownerTm.position;
        var end = start + dir * _skillRange;
        
        _lineRenderer.SetPosition(0, start);
        _lineRenderer.SetPosition(1, end);
    }
}