using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    private IBTComposite _btRoot;
    
    protected virtual void Awake()
    {
        //TODO: 툴에서 제작한 BT를 가져와 설정을 해야함.
        _btRoot = new BTSelector();
        _btRoot.AddNode(new BTAction_Attack())
               .AddNode(new BTAction_Chase());
        
        _btRoot.Execute();
    }
}