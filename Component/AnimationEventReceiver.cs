using R3;
using UnityEngine;

public sealed class AnimationEventReceiver : MonoBehaviour
{
    public Observable<AnimationEvent> OnAttack => _onAttack;
    private readonly ReactiveCommand<AnimationEvent> _onAttack = new();

    private void OnDestroy()
    {
        _onAttack.Dispose();
    }

    private void Attack(AnimationEvent animationEvent)
    {
        _onAttack.Execute(animationEvent);
    }
}