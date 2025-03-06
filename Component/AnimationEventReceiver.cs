using System;
using UnityEngine;

public sealed class AnimationEventReceiver : MonoBehaviour
{
    private IAttacker _attacker;

    private void Awake()
    {
        _attacker = GetComponent<IAttacker>();
    }

    private void Attack(AnimationEvent animationEvent)
    {
        _attacker.IsAttackable = true;
    }
}