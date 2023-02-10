using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MGame.PlayerInputSystem.Point
{
    public struct PointerInput
    {
        public bool Contact;
        public int InputId;
        public Vector2 Position;
        public Vector2? Tilt;
        /// <summary>
        /// Pressure of draw input.
        /// </summary>
        public float? Pressure;

        /// <summary>
        /// Radius of draw input.
        /// </summary>
        public Vector2? Radius;

        /// <summary>
        /// Twist of draw input.
        /// </summary>
        public float? Twist;
    }
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public class PointerInputComposite : InputBindingComposite<PointerInput>
    {
        [InputControl(layout = "Button")]
        public int contact;

        [InputControl(layout = "Vector2")]
        public int position;

        [InputControl(layout = "Vector2")]
        public int tilt;

        [InputControl(layout = "Vector2")]
        public int radius;

        [InputControl(layout = "Axis")]
        public int pressure;

        [InputControl(layout = "Axis")]
        public int twist;

        [InputControl(layout = "Integer")]
        public int inputId;

        public override PointerInput ReadValue(ref InputBindingCompositeContext context)
        {
            var contact = context.ReadValueAsButton(this.contact);
            var pointerId = context.ReadValue<int>(inputId);
            var pressure = context.ReadValue<float>(this.pressure);
            var radius = context.ReadValue<Vector2, Vector2MagnitudeComparer>(this.radius);
            var tilt = context.ReadValue<Vector2, Vector2MagnitudeComparer>(this.tilt);
            var position = context.ReadValue<Vector2, Vector2MagnitudeComparer>(this.position);
            var twist = context.ReadValue<float>(this.twist);

            return new PointerInput
            {
                Contact = contact,
                InputId = pointerId,
                Position = position,
                Tilt = tilt != default ? tilt : (Vector2?)null,
                Pressure = pressure > 0 ? pressure : (float?)null,
                Radius = radius.sqrMagnitude > 0 ? radius : (Vector2?)null,
                Twist = twist > 0 ? twist : (float?)null,
            };
        }

        #if UNITY_EDITOR
        static PointerInputComposite()
        {
            Register();
        }

        #endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            InputSystem.RegisterBindingComposite<PointerInputComposite>();
        }
    }
}
