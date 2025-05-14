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

public interface IAnimatorController
{
    Observable<EAnimState> OnAnimStateExit { get; }
    void SetState(EAnimState state, float value = 0f);
}

public sealed class AnimatorController : 
    IAnimatorController,
    IUnitStateMachineBehaviour
{
    Observable<EAnimState> IAnimatorController.OnAnimStateExit => _onAnimStateExit;
    private readonly ReactiveCommand<EAnimState> _onAnimStateExit = new();
    private EAnimState _curAnimStateType = EAnimState.End;
    private readonly int[] _stateHash = new int[(int)EAnimState.End];
    private readonly Animator _animator;
    
    public AnimatorController(IReadOnlyUnit owner)
    {
        _animator = owner.Tm.GetComponent<Animator>();
    }

    public void Release()
    {
        _onAnimStateExit.Dispose(); 
    }

    public void Initialize()
    {
        InitStateHash();
    }
    
    private void InitStateHash()
    {
        for (var type = EAnimState.Idle; type < EAnimState.End; type++)
        {
            _stateHash[(int)type] = Animator.StringToHash(type.ToString());
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
    
    void IAnimatorController.SetState(EAnimState state, float value)
    {
        if (_curAnimStateType == state)
        {
            return;
        }

        _curAnimStateType = state;
        var stateHash = _stateHash[(int)state];
        
        switch (state)
        {
        case EAnimState.Walk:
            {
                _animator.SetFloat(stateHash, value);
            }
            return;

        default:
            {
                _animator.SetTrigger(stateHash);
                _animator.SetFloat(_stateHash[(int)EAnimState.Walk], 0f);
            }
            break;
        }
    }
    
    void IUnitStateMachineBehaviour.ExecuteAnimStateExit(AnimatorStateInfo stateInfo)
    {
        var animState = GetStateFromHash(stateInfo.shortNameHash);
        _onAnimStateExit.Execute(animState);
        _curAnimStateType = EAnimState.End;
    }
}
