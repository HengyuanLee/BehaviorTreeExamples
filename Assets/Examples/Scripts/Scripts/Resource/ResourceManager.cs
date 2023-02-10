using UnityEngine;

namespace Stealth
{
    public class ResourceManager
    {
        public static T Load<T>(string assetPath) where T : Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
    }
}
