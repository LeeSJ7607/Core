using UnityEngine;

public sealed class AttackController
{
    private readonly IAttacker _owner;
    
    public AttackController(Unit owner)
    {
        _owner = owner;
    }
    
    public bool IsTargetInRange(Vector3 targetPos)
    {
        return true;
    }

    public void Attack(IDefender target)
    {
        target.Hit(0);
    }
}