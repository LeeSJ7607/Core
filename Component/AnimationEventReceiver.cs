using System;
using UnityEngine;

public sealed class AnimationEventReceiver : MonoBehaviour
{
    public Action<AnimationEvent> OnAttack { get; set; }

    private void OnDestroy()
    {
        OnAttack = null;
    }

    private void Attack(AnimationEvent animationEvent)
    {
        OnAttack?.Invoke(animationEvent);
    }
}