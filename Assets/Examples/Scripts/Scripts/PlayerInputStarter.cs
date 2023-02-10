using MGame.PlayerInputSystem.Point;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MGame.PlayerInputSystem
{
    /// <summary>
    /// 捕获玩家输入事件唯一入口，供游戏读取输入作出响应。
    /// </summary>
    public class PlayerInputStarter : MonoBehaviour
    {
        public PlayerInputUIDragRect uiDragRect;
        public Vector2 move { get; private set; }
        public Vector2 look;
        public Vector2 touch0 { get; private set; }

        private PlayerInput m_PlayerInput;


        private void Awake()
        {
            m_PlayerInput = GetComponent<PlayerInput>();
            uiDragRect.onPlayerDragCallback = SetLook;
        }
        void Update()
        {
            move = m_PlayerInput.actions["move"].ReadValue<Vector2>();
            //look = m_PlayerInput.actions["look"].ReadValue<Vector2>();
        }
        public void SetLook(Vector2 look)
        {
            this.look = look * 0.1f;
        }
        public void OnDragEnd()
        {
            this.look = Vector2.zero;
        }
//#if !UNITY_IOS || !UNITY_ANDROID

//        private void OnApplicationFocus(bool hasFocus)
//        {
//            Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
//        }
//#endif
    }
}
