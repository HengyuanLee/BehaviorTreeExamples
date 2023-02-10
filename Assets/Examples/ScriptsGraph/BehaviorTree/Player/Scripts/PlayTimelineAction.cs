using GraphProcessor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayTimelineAction")]
    [NodeName("播放Timeline")]
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
            //从Timeli上找到AnimationClip赋值到SimpleAnimation来集中到一个Graph播放，避免各个Timeline的动画直接无法插值过渡播放衔接。
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
                                //获取轨道上的animation clip资产添加到SimpleAnimation
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
            //播放动画
            if (!string.IsNullOrEmpty(m_AnimName))
            {
                m_SimpleAnimation.CrossFade(m_AnimName, 0.2f);
            }
            //播放Timeline
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
