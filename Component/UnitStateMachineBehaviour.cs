using UnityEngine;

internal interface IUnitStateMachineBehaviour
{
    void ExecuteAnimStateExit(AnimatorStateInfo stateInfo);
}

internal sealed class UnitStateMachineBehaviour : StateMachineBehaviour
{
    private IUnitStateMachineBehaviour _unitStateMachineBehaviour;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        BindUnitStateMachineBehaviour(animator);
    }

    private void BindUnitStateMachineBehaviour(Animator animator)
    {
        if (_unitStateMachineBehaviour != null)
        {
            return;
        }

        if (!animator.TryGetComponent<IReadOnlyUnit>(out var unit))
        {
            Debug.LogError($"{nameof(IReadOnlyUnit)} Component not found");
            return;
        }
        
        _unitStateMachineBehaviour = (IUnitStateMachineBehaviour)unit.AnimatorController;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _unitStateMachineBehaviour.ExecuteAnimStateExit(stateInfo);
    }
}