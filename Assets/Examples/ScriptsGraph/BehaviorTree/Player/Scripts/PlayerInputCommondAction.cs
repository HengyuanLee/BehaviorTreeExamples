using GraphProcessor;
using StarterAssets;
using UnityEngine;
using UnityEngine.Playables;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayerInputCommondAction")]
    [NodeName("ÕÊº“ ‰»Î√¸¡Ó")]
    //[NodeColor(1f, 0.6f, 0.8f)]
    public class PlayerInputCommondAction : BaseActionNode
    {
        public PlayerInputCommandType m_PlayerInputCommandType;
        public bool m_NeedReset;
        public override TaskStatus Tick()
        {
            switch (m_PlayerInputCommandType)
            {
                case PlayerInputCommandType.None:
                    return TaskStatus.Success;
                case PlayerInputCommandType.Walk:
                    if (PlayerInputs.Instance.move != default && !PlayerInputs.Instance.sprint)
                    {
                        return TaskStatus.Success;
                    }
                    break;
                case PlayerInputCommandType.Sprint:
                    if (PlayerInputs.Instance.move != default && PlayerInputs.Instance.sprint)
                    {
                        return TaskStatus.Success;
                    }
                    break;
                case PlayerInputCommandType.Jump:
                    if (PlayerInputs.Instance.jump)
                    {
                        return TaskStatus.Success;
                    }
                    break;
                case PlayerInputCommandType.Skill:
                    if (PlayerInputs.Instance.skill_1)
                    {
                        if (m_NeedReset)
                        {
                            PlayerInputs.Instance.skill_1 = false;
                        }
                        return TaskStatus.Success;
                    }
                    break;
            }
            return TaskStatus.Failure;
        }
    }
}
