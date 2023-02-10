using UnityEngine;

namespace MGame
{
    public class PlayerCameraFollow : MonoBehaviour
    {
        public Transform target;//目标
        public Vector3 targetPosAddition = Vector3.up;//跟随目标视点偏移
        public float rotateSpeed = 52;
        public float desireDist = 6;//期望距离
        public float maxDist = 8;//最大距离
        public float cameraFadeSpeed = 5f;

        private Vector2 m_Rotation;
        private Vector3 m_CurrentLookDir = Vector3.forward;


        public void Look(Vector2 rotate)
        {
            if (rotate.sqrMagnitude < 0.01)
                return;
            var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
            m_Rotation.y += rotate.x * scaledRotateSpeed;
            m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89, 89);

            m_CurrentLookDir = Quaternion.Euler(m_Rotation.x, m_Rotation.y, 0) * Vector3.forward;
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }
            Vector3 targetPos = target.position + targetPosAddition;
            Vector3 newPos = target.position - m_CurrentLookDir * desireDist;
            //不能低于角色位置。要不会穿透地板
            newPos.y = Mathf.Max(newPos.y, target.position.y + targetPosAddition.y);
            transform.position = newPos;// Vector3.Lerp(transform.position, newPos, Time.deltaTime * cameraFadeSpeed);
            transform.LookAt(target.position + targetPosAddition);
        }
    }

}