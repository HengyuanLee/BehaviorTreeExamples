using GraphProcessor;
using UnityEngine;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayAnimationAction")]
    [NodeName("²¥·Å¶¯»­")]
    public class PlayAnimationAction : BaseActionNode
    {
        public AnimationClip m_AnimationClip;
        private SimpleAnimation m_SimpleAnimation;

        public override void Awake()
        {
            m_SimpleAnimation = gameObject.GetComponentInChildren<SimpleAnimation>();
            m_SimpleAnimation.AddClip(m_AnimationClip, m_AnimationClip.name);
        }
        public override TaskStatus Tick()
        {
            m_SimpleAnimation.CrossFade(m_AnimationClip.name, 0.2f);
            return TaskStatus.Success;
        }
    }
}
