using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

[CustomEditor(typeof(PrefabRegistry))]
public class PrefabHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrefabRegistry holder = (PrefabRegistry)target;

        if (GUILayout.Button("从文件夹导入 Prefab"))
        {
            string assetPath = "Assets/art/Prefabs1";

            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { assetPath });

            holder.artList.Clear();

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    holder.artList.Add(prefab);
                }
            }

            EditorUtility.SetDirty(holder);
            Debug.Log($"成功导入 {holder.artList.Count} 个 Prefab");
        }
    }
}
