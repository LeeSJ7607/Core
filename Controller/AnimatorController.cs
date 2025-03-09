using R3;
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
    public Observable<EAnimState> OnAnimStateExit => _onAnimStateExit;
    private readonly ReactiveCommand<EAnimState> _onAnimStateExit = new();
    private readonly int[] _stateHash = new int[(int)EAnimState.End];
    private readonly Animator _animator;
    
    public AnimatorController(Animator animator)
    {
        _animator = animator;
        _animator.AddComponent<AnimationEventReceiver>();
    }

    public void Release()
    {
        _onAnimStateExit.Dispose(); 
    }

    public void Initialize()
    {
        InitStateHash();
    }

    public void OnUpdate()
    {
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime < 1f)
        {
            return;
        }

        var animState = GetStateFromHash(stateInfo.shortNameHash);
        _onAnimStateExit.Execute(animState);
    }
    
    private void InitStateHash()
    {
        var i = 0;
        
        for (var type = EAnimState.Idle; type < EAnimState.End; type++)
        {
            _stateHash[i] = Animator.StringToHash(type.ToString());
            i++;
        }
    }

    private EAnimState GetStateFromHash(int hash)
    {
        for (var i = 0; i < _stateHash.Length; i++)
        {
            if (_stateHash[i] == hash)
            {
                return (EAnimState)i;
            }
        }
        
        Debug.LogError($"Not found state hash: {hash}");
        return EAnimState.End;
    }
    
    public void SetState(EAnimState state, float value = 0f)
    {
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
