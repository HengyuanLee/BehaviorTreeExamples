using GraphProcessor;
using StarterAssets;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/PlayerInput/PlayerSkillAction")]
    [NodeName("Íæ¼Ò¼¼ÄÜ")]
    public class PlayerSkillAction : BaseActionNode
    {
        private const float questTime = 0.6f;
        public float skillTime;


        public override void Awake()
        {
            skillTime = 0;
        }
        public override void Start()
        {
            skillTime = questTime;
        }
        public override TaskStatus Tick()
        {
            if (skillTime > 0)
            {
                skillTime -= UnityEngine.Time.deltaTime;
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}
