using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace MyBehaviorTree
{
    public class GlobalSetting : ScriptableObject
    {
        public List<Object> iconFolders;
        public string[] IconFolders {
            get {
                List<string> list = new List<string>();
                if (iconFolders != null && iconFolders.Count > 0)
                {
                    foreach (var obj in iconFolders)
                    {
                        var assetSelectionPath = AssetDatabase.GetAssetPath(obj);
                        list.Add(assetSelectionPath);
                    }
                }
                return list.ToArray();
            }
        }

        private void OnValidate()
        {
            if (iconFolders != null && iconFolders.Count > 0)
            {
                for (int i = iconFolders.Count-1; i >= 0; i--)
                {
                    var obj = iconFolders[i];
                    var assetSelectionPath = AssetDatabase.GetAssetPath(obj);
                    var isFolder = false;
                    if (!string.IsNullOrEmpty(assetSelectionPath))
                        isFolder = File.GetAttributes(assetSelectionPath).HasFlag(FileAttributes.Directory);
                    if (!isFolder && obj != null)
                    {
                        iconFolders[i] = null;
                        Debug.LogWarning("请拖入文件夹类型");
                    }
                }
            }
        }
    }
}
