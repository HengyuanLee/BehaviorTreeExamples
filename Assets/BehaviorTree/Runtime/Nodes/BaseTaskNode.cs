using UnityEngine;
using GraphProcessor;
using System;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeSize(140, 0)]
    [NodeColor(0.6f, 0.6f, 0.6f)]
    [TaskNodeMenuItem("")]
    public abstract class BaseTaskNode : BaseNode, ITaskNode
    {
        public float Priority => 0f;
        public bool enable = true;
        public string description;
        [System.NonSerialized]
        private bool isFirstTick = true;
        [System.NonSerialized]
        private ulong lastTickFrame;
        [System.NonSerialized]
        internal bool canDoTick = true;
        [System.NonSerialized]
        internal TreeManager ownerTreeManager;
        public TaskStatus LastReturnTaskStatus { get; private set; } = TaskStatus.Inactive;

        public Action OnDoTickCallback;

        public GameObject gameObject => ownerTreeManager.gameObject;
        public Transform transform => ownerTreeManager.transform;
        protected virtual void BeforeTick() { }
        protected virtual void AfterTick() { }
        public virtual TaskStatus DoTick()
        {
            if (isFirstTick)
            {
                isFirstTick = false;
                Awake();//awake只执行一次。
                Start();
            }
            else if (lastTickFrame+1 != ownerTreeManager.FrameIndex && LastReturnTaskStatus != TaskStatus.Running)
            {
                Start();
            }
            BeforeTick();
            LastReturnTaskStatus = Tick();
            lastTickFrame = ownerTreeManager.FrameIndex;
            OnDoTickCallback?.Invoke();
            AfterTick();
            return LastReturnTaskStatus;
        }
        public void ResetStatus()
        {
            LastReturnTaskStatus = TaskStatus.Inactive;
        }
        public virtual void Awake()
        {
        }
        public virtual void Start()
        {
        }
        public virtual TaskStatus Tick()
        {
            return TaskStatus.Success;
        }

        public virtual bool CanDoTick()
        {
            return canDoTick && enable;
        }
    }
}
