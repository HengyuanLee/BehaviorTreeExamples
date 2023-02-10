using UnityEngine;
using UnityEditor;
using System.IO;

namespace MyBehaviorTree
{
    public class GlobalSettingRefrence : ScriptableObject
    {

        public static GlobalSettingRefrence Instance
        {
            get {
                if (instance == null)
                {
                    instance = new GlobalSettingRefrence();
                }
                return instance;
            }
        }
        private static GlobalSettingRefrence instance;

        public GlobalSetting GlobalSetting => globalSetting;
        [SerializeField]
        private GlobalSetting globalSetting;
        [SerializeField]
        private Object settingAssetFolder;

        [MenuItem("Assets/Create/MyBehaviorTree/Setting", false, 10)]
        public static void CreateSetting()
        {
            var assetSelectionPath = Instance.GetSettingAssetFolder();
            if (assetSelectionPath == null)
            {
                Debug.LogWarning("ÇëÑ¡ÔñÂ·¾¶");
            }
            else
            {
                var setting = ScriptableObject.CreateInstance<GlobalSetting>();
                string assetFile = assetSelectionPath + "/GlobalSetting.asset";
                if (!File.Exists(assetFile)) {
                    AssetDatabase.CreateAsset(setting, assetFile);
                    Instance.globalSetting = AssetDatabase.LoadAssetAtPath<GlobalSetting>(assetFile);
                }
            }
        }
        private string GetSettingAssetFolder()
        {
            return GetObjFolderString(settingAssetFolder);
        }
        private string GetObjFolderString(Object folderObj)
        {
            var assetSelectionPath = AssetDatabase.GetAssetPath(folderObj);
            var isFolder = false;
            if (!string.IsNullOrEmpty(assetSelectionPath))
                isFolder = File.GetAttributes(assetSelectionPath).HasFlag(FileAttributes.Directory);
            if (isFolder)
            {
                return assetSelectionPath;
            }
            else
            {
                return null;
            }
        }
    }
}
