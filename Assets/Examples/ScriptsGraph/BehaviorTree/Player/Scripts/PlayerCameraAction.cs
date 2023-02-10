using GraphProcessor;
using MGame;
using StarterAssets;
using UnityEngine;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayerInput/PlayerCameraAction")]
    [NodeName("������")]
    public class PlayerCameraAction : BaseActionNode
    {

        public override void Awake()
        {
            Camera.main.gameObject.AddComponent<PlayerCameraFollow>().target = gameObject.transform;
        }
        public override TaskStatus Tick()
        {
            Camera.main.GetComponent<PlayerCameraFollow>().Look(PlayerInputs.Instance.look);
            return TaskStatus.Success;
        }

    }
}

