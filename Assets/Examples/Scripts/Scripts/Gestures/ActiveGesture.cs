using UnityEngine;

namespace MGame.PlayerInputSystem.Point
{
    internal sealed class ActiveGesture
    {
        public int InputId;
        public readonly double StartTime;
        public double EndTime;
        public readonly Vector2 StartPosition;
        public Vector2 PreviousPosition;
        public Vector2 EndPosition;
        public int Samples;
        public float SwipeDirectionSameness;
        public float TravelDistance;


        private Vector2 accumulatedNormalized;

        public ActiveGesture(int inputId, Vector2 startPosition, double startTime)
        {
            InputId = inputId;
            EndTime = StartTime = startTime;
            EndPosition = StartPosition = startPosition;
            Samples = 1;
            SwipeDirectionSameness = 1;
            accumulatedNormalized = Vector2.zero;
        }

        public void SubmitPoint(Vector2 position, double time)
        {
            Vector2 toNewPosition = position - EndPosition;
            float distanceMoved = toNewPosition.magnitude;

            EndTime = time;

            if (Mathf.Approximately(distanceMoved, 0))
            {
                return;
            }

            toNewPosition /= distanceMoved;

            Samples++;
            Vector2 toNewEndPosition = (position - StartPosition).normalized;

            PreviousPosition = EndPosition;
            EndPosition = position;

            accumulatedNormalized += toNewPosition;

            SwipeDirectionSameness = Vector2.Dot(toNewEndPosition, accumulatedNormalized / (Samples - 1));

            TravelDistance += distanceMoved;
        }
    }
}
