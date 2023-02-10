using GraphProcessor;
using StarterAssets;
using UnityEngine;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayerInput/PlayerInputRunningAction")]
    [NodeName("玩家输入执行中...")]
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