using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MGame.PlayerInputSystem.Point
{
    public class GestureController : MonoBehaviour
    {
        public Action<Vector2> onDragCallback;
        public Action<Vector2> onOnPressedCallback;
        public Action OnDragEndCallback;
        public Action OnPointerOnUGUICallback;

        [SerializeField]
        private PointerInputManager inputManager;

        [SerializeField]
        private float maxTapDuration = 0.2f;

        [SerializeField]
        private float maxTapDrift = 5.0f;

        [Header("Debug"), SerializeField]
        private Text label;

        private readonly Dictionary<int, ActiveGesture> activeGestures = new Dictionary<int, ActiveGesture>();


        protected virtual void Awake()
        {
            inputManager.Pressed += OnPressed;
            inputManager.Dragged += OnDragged;
            inputManager.Released += OnReleased;
        }

        private bool IsValidTap(ref ActiveGesture gesture)
        {
            return gesture.TravelDistance <= maxTapDrift &&
                (gesture.EndTime - gesture.StartTime) <= maxTapDuration;
        }

        private void OnPressed(PointerInput input, double time)
        {
            Debug.Assert(!activeGestures.ContainsKey(input.InputId));

            var newGesture = new ActiveGesture(input.InputId, input.Position, time);
            activeGestures.Add(input.InputId, newGesture);

            //if (IsPointerOnUGUI() == false)
            {
                onOnPressedCallback?.Invoke(newGesture.EndPosition - newGesture.StartPosition);
            }

            DebugInfo(newGesture);
        }

        private void OnDragged(PointerInput input, double time)
        {
            if (!activeGestures.TryGetValue(input.InputId, out var existingGesture))
            {
                return;
            }

            existingGesture.SubmitPoint(input.Position, time);

            //if (IsPointerOnUGUI() == false)
            //{
            //    onDragCallback?.Invoke(existingGesture.EndPosition - existingGesture.StartPosition);
            //}
            //else {
            //    OnPointerOnUGUICallback?.Invoke();
            //}

            DebugInfo(existingGesture);
        }

        private void OnReleased(PointerInput input, double time)
        {
            if (!activeGestures.TryGetValue(input.InputId, out var existingGesture))
            {
                return;
            }

            activeGestures.Remove(input.InputId);
            existingGesture.SubmitPoint(input.Position, time);

            //if (IsPointerOnUGUI() == false)
            {
                OnDragEndCallback?.Invoke();
            }

            DebugInfo(existingGesture);
        }

        private void DebugInfo(ActiveGesture gesture)
        {
            if (label == null) return;

            var builder = new StringBuilder();

            builder.AppendFormat("ID: {0}", gesture.InputId);
            builder.AppendLine();
            builder.AppendFormat("Start Position: {0}", gesture.StartPosition);
            builder.AppendLine();
            builder.AppendFormat("Position: {0}", gesture.EndPosition);
            builder.AppendLine();
            builder.AppendFormat("Duration: {0}", gesture.EndTime - gesture.StartTime);
            builder.AppendLine();
            builder.AppendFormat("Sameness: {0}", gesture.SwipeDirectionSameness);
            builder.AppendLine();
            builder.AppendFormat("Travel distance: {0}", gesture.TravelDistance);
            builder.AppendLine();
            builder.AppendFormat("Samples: {0}", gesture.Samples);
            builder.AppendLine();
            builder.AppendFormat("Realtime since startup: {0}", Time.realtimeSinceStartup);
            builder.AppendLine();
            builder.AppendFormat("Starting Timestamp: {0}", gesture.StartTime);
            builder.AppendLine();
            builder.AppendFormat("Ending Timestamp: {0}", gesture.EndTime);
            builder.AppendLine();

            label.text = builder.ToString();

            var worldStart = Camera.main.ScreenToWorldPoint(gesture.StartPosition);
            var worldEnd = Camera.main.ScreenToWorldPoint(gesture.EndPosition);

            worldStart.z += 5;
            worldEnd.z += 5;
        }


//        private bool IsPointerOnUGUI()
//        {
//#if IPHONE || ANDROID
//			if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
//#else
//            if (EventSystem.current.IsPointerOverGameObject())
//#endif
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
    }
}
