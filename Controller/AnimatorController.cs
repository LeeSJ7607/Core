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
    
    public AnimatorController(Animator animator)
    {
        _animator = animator;
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
    
    public void SetState(EAnimState state, float value = 0f)
    {
        
    }
}
