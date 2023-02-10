using GraphProcessor;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayerInput/PlayerJumpAction")]
    [NodeName("Íæ¼ÒÌøÔ¾")]
    public class PlayerJumpAction : BaseActionNode
    {
        public float jumpTime;

        public SimpleAnimation m_Anim;
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        private float _verticalVelocity;

        public override void Awake()
        {
        }

        public override TaskStatus Tick()
        {

            if (Grounded)
            {
                // Jump
                if (PlayerInputs.Instance.jump)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(-50 * JumpHeight * Gravity);
                }
            }
            else
            {
                _verticalVelocity = Mathf.Max(_verticalVelocity + 5 * Gravity * Time.deltaTime, Gravity);
                // if we are not grounded, do not jump
                PlayerInputs.Instance.jump = false;
            }
            Vector3 pos = transform.position;
            pos += new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f);
            pos.y = Mathf.Max(0, pos.y);
            transform.position = pos;

            Grounded = transform.position.y <= 0;
            if (!Grounded)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
    }
}
