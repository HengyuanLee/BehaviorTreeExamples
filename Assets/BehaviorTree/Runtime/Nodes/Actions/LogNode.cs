
using GraphProcessor;
using UnityEngine;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/Unity/Log")]
    [NodeIcon("DarkLogIcon.png")]
    [NodeName("»’÷æ")]
    public class LogNode : BaseActionNode
    {
        public string log = "log";
        public LogType logType = LogType.Log;
        public TaskStatus returnTaskStatus = TaskStatus.Success;

        public float utility;

        public override float GetUtility()
        {
            return utility;
        }

        public override TaskStatus Tick()
        {
            if (!string.IsNullOrEmpty(log))
            {
                switch (logType)
                {
                    case LogType.Log:
                        Debug.Log(log);
                        break;
                    case LogType.Warning:
                        Debug.LogWarning(log);
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                    case LogType.Assert:
                        Debug.LogError(log);
                        break;
                }
            }
            return returnTaskStatus;
        }
    }
}