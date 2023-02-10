using GraphProcessor;
using MGame;
using StarterAssets;
using UnityEngine;
namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayerInput/PlayerRotationAction")]
    [NodeName("玩家朝向")]
    public class PlayerRotationAction : BaseActionNode
    {
        public float m_RotaSpeed = 20f;
        public override TaskStatus Tick()
        {
            Move(PlayerInputs.Instance.move);
            return TaskStatus.Success;
        }
        private void Move(Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                //移动插值
                var move = Quaternion.Euler(0, Mathf.Abs(Camera.main.transform.eulerAngles.y), 0) * new Vector3(direction.x, 0, direction.y);
                //转身插值
                Quaternion newQ = Quaternion.FromToRotation(Vector3.forward, move);
                newQ = Quaternion.Euler(0, newQ.eulerAngles.y, 0);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, newQ, Time.deltaTime * m_RotaSpeed);
            }
        }
    }
}