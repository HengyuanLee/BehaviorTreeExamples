using GraphProcessor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayTimelineAction")]
    [NodeName("����Timeline")]
    [NodeColor(1f, 0.6f, 0.8f)]
    public class PlayTimelineAction : BaseActionNode
    {
        public TaskStatus TaskStatus;
        public string m_PlayableDirectorName;
        private PlayableDirector m_PlayableDirector;
        private float time = 0;
        private SimpleAnimation m_SimpleAnimation;
        private string m_AnimName;
        public override void Awake()
        {
            //��Timeli���ҵ�AnimationClip��ֵ��SimpleAnimation�����е�һ��Graph���ţ��������Timeline�Ķ���ֱ���޷���ֵ���ɲ����νӡ�
            m_PlayableDirector = transform.Find(m_PlayableDirectorName).GetComponent<PlayableDirector>();
            var animtor = gameObject.GetComponentInChildren<Animator>();
            m_SimpleAnimation = gameObject.GetComponentInChildren<SimpleAnimation>();
            foreach (PlayableBinding bd in m_PlayableDirector.playableAsset.outputs)
            {
                if (bd.sourceObject is AnimationTrack track)
                {
                    //track.muted = false;
                    var bindingObj = m_PlayableDirector.GetGenericBinding(bd.sourceObject);
                    if (bindingObj == animtor)
                    {
                        foreach (var clip in track.GetClips())
                        {
                            var asset = clip.asset as AnimationPlayableAsset;
                            if (asset != null)
                            {
                                //��ȡ����ϵ�animation clip�ʲ���ӵ�SimpleAnimation
                                m_AnimName = asset.clip.name;
                                m_SimpleAnimation.AddClip(asset.clip, m_AnimName);
                            }
                        }
                    }
                }
            }
        }

        public override void Start()
        {
            time = 0;
        }

        public override TaskStatus Tick()
        {
            //���Ŷ���
            if (!string.IsNullOrEmpty(m_AnimName))
            {
                m_SimpleAnimation.CrossFade(m_AnimName, 0.2f);
            }
            //����Timeline
            time += Time.deltaTime;
            if (m_PlayableDirector.extrapolationMode == DirectorWrapMode.Loop && time >= m_PlayableDirector.duration)
            {
                time = 0;
            }
            m_PlayableDirector.time = time;
            m_PlayableDirector.Evaluate();
            return TaskStatus;
        }
    }
}
