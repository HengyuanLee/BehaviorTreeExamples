using GraphProcessor;
using UnityEngine;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/Stealth/LoadAssetAction")]
    [NodeName("×ÊÔ´¼ÓÔØ")]
    public class LoadAssetAction : BaseActionNode
    {
        public string assetPath;

        public override bool isRenamable => true;

        public override TaskStatus Tick()
        {
            var asset = Stealth.ResourceManager.Load<GameObject>(assetPath);
            if (asset != null)
            {
                GameObject.Instantiate(asset);
            }
            return TaskStatus.Success;
        }
    }
}