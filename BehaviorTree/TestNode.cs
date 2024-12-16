// DebugLogNode.cs
// 05-13-2022
// James LaFritz

using UnityEngine;

namespace GraphViewBehaviorTree.Nodes
{
    /// <summary>
    /// <see cref="ActionNode"/> that logs a message.
    /// </summary>
    [System.Serializable]
    public class DebugLogNode : BTNode
    {
        /// <value>
        /// The Message to Log On Start.
        /// Empty will not Log a message.
        /// </value>
        [SerializeField] private string onStartMessage;

        /// <value>
        /// The Message to Log On Stop.
        /// Empty will not Log a message.
        /// </value>
        [SerializeField] private string onStopMessage;

        /// <value>
        /// The Message to Log On Start.
        /// Empty will not Log a message.
        /// </value>
        [SerializeField] private string onUpdateMessage;


        public override Status Update()
        {
            Debug.Log("asdf");
            return Status.Running;
        }
    }
}