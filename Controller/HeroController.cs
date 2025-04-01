using UnityEngine;

public sealed class HeroController
{
    private readonly IReadOnlyUnit _owner;
    private readonly MoveController _moveController;
    private readonly Camera _mainCam;
    
    public HeroController(IReadOnlyUnit owner)
    {
        _owner = owner;
        _moveController = new MoveController(owner);
        _mainCam = Camera.main ?? throw new System.NullReferenceException("Main camera is missing.");
    }

    public void OnUpdate()
    {
        UpdateCameraPos();
        HandleMovement();
    }

    private void UpdateCameraPos()
    {
        //TODO: 시네머신 카메라 사용하면 매직 넘버 사용하지 않아도 됨.
        var pos = _owner.Tm.position;
        pos.x -= 7f;
        pos.y = 10f;
        
        _mainCam.transform.position = pos;
    }

    private void HandleMovement()
    {
        if (TryGetHitPoint(out var pos))
        {
            _moveController.MoveTo(pos);
        }
        else
        {
            _moveController.GetMoveState();
        }
    }

    private bool TryGetHitPoint(out Vector3 pos)
    {
        pos = default;
        
        if (!Input.GetMouseButtonDown(0))
        {
            return false;
        }
        
        var mask = LayerMask.GetMask("Floor");
        var ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, mask))
        {
            return false;
        }

        pos = hit.point;
        pos.y = _owner.Tm.position.y;
        return true;
    }
}