using UnityEngine;

//TODO: ScriptableObject 왜 상속 받지? (런타임에서 값을 변경해야할 경우 필요할 수도 있음)
//TODO: 필요한 클래스인가?? 필요하면 리펙토링 필수.
[CreateAssetMenu(fileName = "BehaviorTree", menuName = "Core/BehaviorTree")]
internal sealed class BehaviorTree : ScriptableObject
{
    public BTNode RootNode;
    public BTNode.Status TreeStatus;
    
    public BTNode.Status Update()
    {
        var hasRootNode = RootNode != null;

        if (!hasRootNode)
        {
            Debug.LogWarning($"{name} needs a root node in order to properly run. Please add one.", this);
            return BTNode.Status.Failure;
        }

        if (TreeStatus == BTNode.Status.Running)
        {
            TreeStatus = RootNode.Update();
        }

        return TreeStatus;
    }
}