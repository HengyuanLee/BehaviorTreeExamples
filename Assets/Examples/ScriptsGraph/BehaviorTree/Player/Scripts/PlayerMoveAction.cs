using GraphProcessor;
using StarterAssets;
using UnityEngine;
namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayerInput/PlayerMoveAction")]
    [NodeName("玩家移动")]
    public class PlayerMoveAction : BaseActionNode
    {
        public TaskStatus m_MoveSuccess = TaskStatus.Success;
        public TaskStatus m_MoveFailure = TaskStatus.Failure;
        public float m_Speed = 2f;
        public float m_WalkSpeed = 1.5f;
        public float m_SprintSpeed = 5f;
        public float m_RotaSpeed = 20f;

        public override TaskStatus Tick()
        {
            if (PlayerInputs.Instance.move != Vector2.zero)
            {
                m_Speed = PlayerInputs.Instance.sprint ? m_SprintSpeed : m_WalkSpeed;
                Move(PlayerInputs.Instance.move);
                if (transform.position.y == 0)
                {
                    if (PlayerInputs.Instance.sprint)
                    {
                        //m_Anim.CrossFade("run", 0.2f);
                    }
                    else
                    {
                        //m_Anim.CrossFade("walk", 0.2f);
                    }
                }
                return m_MoveSuccess;
            }
            return m_MoveFailure;
        }
        private void Move(Vector2 direction)
        {

            if (direction != Vector2.zero)
            {
                //移动插值
                var scaledMoveSpeed = m_Speed * Time.deltaTime;
                var move = Quaternion.Euler(0, Mathf.Abs(Camera.main.transform.eulerAngles.y), 0) * new Vector3(direction.x, 0, direction.y);
                Vector3 newPos = transform.position + move * scaledMoveSpeed;
                newPos.y = transform.position.y;
                if (transform.position != newPos)
                {
                    transform.position = newPos;
                }
            }
        }
    }
}