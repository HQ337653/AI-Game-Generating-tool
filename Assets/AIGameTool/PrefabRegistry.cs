using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局 PrefabRegistry 管理 prefab root、子对象以及 scene 对象
/// 支持跨 prefab / prefab 内 / prefab child 对象和组件引用
/// </summary>
public class PrefabRegistry : MonoBehaviour
{


    public static PrefabRegistry Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<PrefabRegistry>();
            }
            return _Instance;
        }
    }

    [SerializeField]
    GameObject virtualCamera;
    [SerializeField]
    GameObject staticSceenItems, DynamicSceenItems;
    public string GetAllArtPrefabNames()
    {
        if (artList == null || artList.Count == 0)
            return string.Empty;

        List<string> names = new List<string>();

        foreach (var go in artList)
        {
            if (go == null) continue;
            names.Add(go.name);
        }

        return string.Join("(prefab), ", names)+ "(prefab)";
    }

    public string GetAllArtPrefabID()
    {
        if (idMap == null || idMap.Count == 0)
            return string.Empty;

        List<string> names = new List<string>();

        foreach (var go in idMap)
        {
            if (go == null) continue;
            names.Add(go.Id);
        }

        return string.Join(", ", names);
    }
    private static PrefabRegistry _Instance;

    [Tooltip("美术 prefab 原型列表（仅作为 Instantiate 原型）")]
    public List<GameObject> artList = new List<GameObject>();

    [SerializeField] private List<GameObjectId> idMap = new List<GameObjectId>();
    [SerializeField] private List<GameObjectId> DefauIDIdMap = new List<GameObjectId>();

    // ------------------------------------------------------------------
    // 美术资源查找
    // ------------------------------------------------------------------

    /// <summary>
    /// 根据美术资源名字查找 prefab 原型（不实例化）
    /// </summary>
    public GameObject GetArtPrefabByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        foreach (var go in artList)
        {
            if (go == null) continue;

            if (string.Equals(go.name, name, StringComparison.OrdinalIgnoreCase))
                return go;
        }

        return null;
    }



    // ------------------------------------------------------------------
    // 注册逻辑
    // ------------------------------------------------------------------

    /// <summary>
    /// 注册 prefab root，并递归注册所有子对象
    /// ?? 不修改 Active 状态
    /// </summary>
    public void RegisterPrefabRoot(string rootId, GameObject prefabRoot)
    {
        if (prefabRoot == null || string.IsNullOrEmpty(rootId))
            return;

        // Register the root prefab
        idMap.Add(new GameObjectId(rootId, prefabRoot));

    }
    [ContextMenu("Debug Print All GameObject Name-ID")]
    public void DebugPrintAllGameObjectNameId()
    {
        Debug.Log("===== PrefabRegistry: Global Objects =====");

        foreach (var item in idMap)
        {
            if (item.GameObject == null)
            {
                Debug.Log($"[GLOBAL] id={item.Id} name=<null>");
            }
            else
            {
                Debug.Log($"[GLOBAL] id={item.Id} name={item.GameObject.name}");
            }
        }

        Debug.Log("===== End Print =====");
    }


    // ------------------------------------------------------------------
    // 查询接口
    // ------------------------------------------------------------------

    public GameObject GetObject(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        var go = FindObjectById(id);
        return go;
    }

    public T GetComponent<T>(string id) where T : Component
    {
        var go = GetObject(id);
        if (go == null) return null;
        return go.GetComponent<T>();
    }

    public bool IsRegistered(string id)
    {
        return FindObjectById(id) != null;
    }

    /// <summary>
    /// 查找对象 ID
    /// </summary>
    public GameObject FindObjectById(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        // 先在主表找
        var item = idMap.Find(x => x.Id == id);
        if (item != null)
            return item.GameObject;

        // 再在默认表找
        item = DefauIDIdMap.Find(x => x.Id == id);
        return item?.GameObject;
    }


    // ------------------------------------------------------------------
    // Instantiate（保持 Active）
    // ------------------------------------------------------------------

    /// <summary>
    /// 根据 prefabId 创建实例
    /// ?? 实例保持 active，由调用方决定何时关闭
    /// </summary>
    public GameObject InstantiatePrefab(string id, Transform parent = null)
    {
        var prefab = GetObject(id);
        if (prefab == null)
        {
            Debug.LogError($"PrefabRegistry: prefab not found: {id}");
            return null;
        }

        var go = Instantiate(prefab, parent);
        go.name = prefab.name;

        // 保持 active（非常关键）
        if (!go.activeSelf)
            go.SetActive(true);

        string instancePrefix = id + "_inst";
        idMap.Add(new GameObjectId(instancePrefix, go));

        return go;
    }
}

/// <summary>
/// 存储 GameObject 的 ID 和对象本身
/// </summary>
[System.Serializable]
public class GameObjectId
{
    public string Id;
    public GameObject GameObject;

    public GameObjectId(string id, GameObject go)
    {
        Id = id;
        GameObject = go;
    }
}
