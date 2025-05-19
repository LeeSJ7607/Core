using R3;
using UnityEngine;

public enum eAnimState
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
    Observable<eAnimState> OnAnimStateExit { get; }
    void SetState(eAnimState state, float value = 0f);
}

public sealed class AnimatorController : 
    IAnimatorController,
    IUnitStateMachineBehaviour
{
    Observable<eAnimState> IAnimatorController.OnAnimStateExit => _onAnimStateExit;
    private readonly ReactiveCommand<eAnimState> _onAnimStateExit = new();
    private eAnimState _curAnimStateType = eAnimState.End;
    private readonly int[] _stateHash = new int[(int)eAnimState.End];
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
        for (var type = eAnimState.Idle; type < eAnimState.End; type++)
        {
            _stateHash[(int)type] = Animator.StringToHash(type.ToString());
        }
    }

    private eAnimState GetStateFromHash(int hash)
    {
        for (var i = 0; i < _stateHash.Length; i++)
        {
            if (_stateHash[i] == hash)
            {
                return (eAnimState)i;
            }
        }
        
        Debug.LogError($"Not found state hash: {hash}");
        return eAnimState.End;
    }
    
    void IAnimatorController.SetState(eAnimState state, float value)
    {
        if (_curAnimStateType == state)
        {
            return;
        }

        _curAnimStateType = state;
        var stateHash = _stateHash[(int)state];
        
        switch (state)
        {
        case eAnimState.Walk:
            {
                _animator.SetFloat(stateHash, value);
            }
            return;

        default:
            {
                _animator.SetTrigger(stateHash);
                _animator.SetFloat(_stateHash[(int)eAnimState.Walk], 0f);
            }
            break;
        }
    }
    
    void IUnitStateMachineBehaviour.ExecuteAnimStateExit(AnimatorStateInfo stateInfo)
    {
        var animState = GetStateFromHash(stateInfo.shortNameHash);
        _onAnimStateExit.Execute(animState);
        _curAnimStateType = eAnimState.End;
    }
}
