using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MGame.PlayerInputSystem
{
    public class ThirdPersonController : MonoBehaviour
    {
        public Transform controllTarget;
        private float moveSpeed = 6f;
        private float rotateSpeed = 20f;
        private PlayerInputs m_PlayerInputStarter;
        private PlayerCameraFollow m_PlayerCameraFollow;

        //private Animator m_ControllTargetAnimator;

        public void Awake()
        {
            m_PlayerInputStarter = gameObject.GetComponent<PlayerInputs>();
            //m_ControllTargetAnimator = controllTarget.GetComponent<Animator>();
        }

        public void Update()
        {
            if (m_PlayerCameraFollow == null)
            {
                GameObject camObj = Camera.main.gameObject;
                if (camObj != null)
                {
                    m_PlayerCameraFollow = camObj.AddComponent<PlayerCameraFollow>();
                    m_PlayerCameraFollow.target = controllTarget;
                }
            }
            Move(m_PlayerInputStarter.move);
            Look(m_PlayerInputStarter.look);
        }
        private void Look(Vector2 rotate)
        {
            m_PlayerCameraFollow.Look(rotate);
        }
        enum AnimState {
            Unknown,
            Idle,
            Run
            }
        AnimState animState;
        private void Move(Vector2 direction)
        {
            if (m_PlayerCameraFollow == null)
            {
                return;
            }
            if (direction == Vector2.zero)
            {
                if (animState != AnimState.Idle)
                {
                    animState = AnimState.Idle;
                    //m_ControllTargetAnimator.SetBool("Idle", true);
                    //m_ControllTargetAnimator.SetBool("Run", false);
                }
            }
            else
            {
                //移动插值
                var scaledMoveSpeed = moveSpeed * Time.deltaTime;
                var move = Quaternion.Euler(0, Mathf.Abs(m_PlayerCameraFollow.transform.eulerAngles.y), 0) * new Vector3(direction.x, 0, direction.y);
                Vector3 newPos = controllTarget.position + move * scaledMoveSpeed;
                if (controllTarget.position != newPos)
                {
                    controllTarget.position = newPos;
                    if (animState != AnimState.Run)
                    {
                        animState = AnimState.Run;
                        //m_ControllTargetAnimator.SetBool("Idle", false);
                        //m_ControllTargetAnimator.SetBool("Run", true);
                    }
                }
                //转身插值
                Quaternion newQ = Quaternion.FromToRotation(Vector3.forward, move);
                newQ = Quaternion.Euler(0, newQ.eulerAngles.y, 0);
                controllTarget.localRotation = Quaternion.Lerp(controllTarget.localRotation, newQ, Time.deltaTime * rotateSpeed);
            }
        }
    }
}