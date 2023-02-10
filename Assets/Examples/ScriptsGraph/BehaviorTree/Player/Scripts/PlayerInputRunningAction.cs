using GraphProcessor;
using StarterAssets;
using UnityEngine;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayerInput/PlayerInputRunningAction")]
    [NodeName("�������ִ����...")]
    public class PlayerInputRunningAction : BaseActionNode
    {

        public override TaskStatus Tick()
        {
            if (PlayerInputs.Instance.move != Vector2.zero ||
                PlayerInputs.Instance.sprint ||
                PlayerInputs.Instance.jump ||
                PlayerInputs.Instance.skill_1
               )
            {
                //PlayerInputs.Instance.sprint = false;
                PlayerInputs.Instance.jump = false;
                PlayerInputs.Instance.skill_1 = false;
            }
            return TaskStatus.Failure;

        }
    }
}