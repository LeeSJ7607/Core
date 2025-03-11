using R3;
using UnityEngine;

//TODO: 구조 한번 생각해봐야함.
public sealed class AnimationEventReceiver : MonoBehaviour
{
    public Observable<AnimationEvent> OnAttack => _onAttack;
    private readonly ReactiveCommand<AnimationEvent> _onAttack = new();

    private void OnDisable()
    {
        _onAttack.Dispose();
    }

    private void Attack(AnimationEvent animationEvent)
    {
        _onAttack.Execute(animationEvent);
    }
}