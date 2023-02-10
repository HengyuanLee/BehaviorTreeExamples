using UnityEngine;
using UnityEditor;

namespace MGame.Resource
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance;
        private void Awake()
        {
            Instance = this;
        }
    }
}
