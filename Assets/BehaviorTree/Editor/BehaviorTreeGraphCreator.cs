
using UnityEditor;
using UnityEngine;

namespace MyBehaviorTree
{
    public class BehaviorTreeGraphCreator
    {
        [MenuItem("Assets/Create/MyBehaviorTree/New MyBehaviorTree", false, 10)]
        public static void Create()
        {
            var graph = ScriptableObject.CreateInstance<BehaviorTreeGraph>();
            ProjectWindowUtil.CreateAsset(graph, "MyBehaviorTree.asset");
        }
    }
}
