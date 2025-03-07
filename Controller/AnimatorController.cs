using UnityEngine;

public enum EAnimState
{
    Idle,
    Walk,
    Attack,
    Hit,
    Die,
    
    End,
}

public sealed class AnimatorController
{
    private readonly int[] _stateHash = new int[(int)EAnimState.End];
    private readonly Animator _animator;
    //private EAnimState _curAnimState;
    
    public AnimatorController(Animator animator)
    {
        _animator = animator;
        _animator.AddComponent<AnimationEventReceiver>();
        InitStateHash();
    }
    
    // public void OnUpdate()
    // {
    //     if (_curAnimState != EAnimatorState.Die)
    //     {
    //         return;
    //     }
    //     
    //     if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
    //     {
    //         return;
    //     }
    //
    //     _onStateExit.Execute(_curAnimState);
    // }

    private void InitStateHash()
    {
        var i = 0;
        
        for (var type = EAnimState.Idle; type < EAnimState.End; type++)
        {
            _stateHash[i] = Animator.StringToHash(type.ToString());
            i++;
        }
    }
    
    public void SetState(EAnimState state, float value = 0f)
    {
        //_curAnimState = state;
        var stateHash = _stateHash[(int)state];

        if (state == EAnimState.Walk)
        {
            _animator.SetFloat(stateHash, value);
        }
        else
        {
            _animator.SetTrigger(stateHash);
        }
    }
}
