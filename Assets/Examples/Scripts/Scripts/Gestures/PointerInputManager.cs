using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MGame.PlayerInputSystem.Point
{
    public class PointerInputManager : MonoBehaviour
    {
        public event Action<PointerInput, double> Pressed;
        public event Action<PointerInput, double> Dragged;
        public event Action<PointerInput, double> Released;

        private bool m_Dragging;
        private PointerControls m_Controls;

        [SerializeField] private bool m_UseMouse;
        [SerializeField] private bool m_UsePen;
        [SerializeField] private bool m_UseTouch;

        protected virtual void Awake()
        {
            m_Controls = new PointerControls();

            m_Controls.pointer.point.performed += OnAction;
            m_Controls.pointer.point.canceled += OnAction;

            SyncBindingMask();
        }

        protected virtual void OnEnable()
        {
            m_Controls?.Enable();
        }

        protected virtual void OnDisable()
        {
            m_Controls?.Disable();
        }

        protected void OnAction(InputAction.CallbackContext context)
        {
            var control = context.control;
            var device = control.device;

            var isMouseInput = device is Mouse;
            var isPenInput = !isMouseInput && device is Pen;

            var drag = context.ReadValue<PointerInput>();
            if (isMouseInput)
                drag.InputId = PointerInputModule.kMouseLeftId;
            else if (isPenInput)
                drag.InputId = int.MinValue;//µÈÓÚPenInputId

            if (drag.Contact && !m_Dragging)
            {
                Pressed?.Invoke(drag, context.time);
                m_Dragging = true;
            }
            else if (drag.Contact && m_Dragging)
            {
                Dragged?.Invoke(drag, context.time);
            }
            else
            {
                Released?.Invoke(drag, context.time);
                m_Dragging = false;
            }
        }

        private void SyncBindingMask()
        {
            if (m_Controls == null)
                return;

            if (m_UseMouse && m_UsePen && m_UseTouch)
            {
                m_Controls.bindingMask = null;
                return;
            }

            m_Controls.bindingMask = InputBinding.MaskByGroups(new[]
            {
                m_UseMouse ? "Mouse" : null,
                m_UsePen ? "Pen" : null,
                m_UseTouch ? "Touch" : null
            });
        }

        private void OnValidate()
        {
            SyncBindingMask();
        }
    }
}
