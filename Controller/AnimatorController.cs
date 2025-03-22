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
    private readonly int[] _stateHash = new int[(int)EAnimState.End];
    private readonly Animator _animator;
    private readonly CompositeDisposable _disposable = new();
    
    public AnimatorController(IReadOnlyUnit owner)
    {
        _animator = owner.Tm.GetComponent<Animator>();
        
        //TODO: 릴리즈 방식을 다르게 해야할까?
        owner.OnRelease
             .Subscribe(_ => Release())
             .AddTo(_disposable);
    }

    private void Release()
    {
        _disposable.Dispose();
        _onAnimStateExit.Dispose(); 
    }

    public void Initialize()
    {
        InitStateHash();
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
    
    void IAnimatorController.SetState(EAnimState state, float value)
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
    
    void IUnitStateMachineBehaviour.ExecuteAnimStateExit(AnimatorStateInfo stateInfo)
    {
        var animState = GetStateFromHash(stateInfo.shortNameHash);
        _onAnimStateExit.Execute(animState);
    }
}
