using Cinemachine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PrefabJsonExecutor : MonoBehaviour
{
    public static PrefabJsonExecutor Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<PrefabJsonExecutor>();
            return _Instance;
        }
    }

    public static PrefabJsonExecutor _Instance;

    [TextArea(20, 40)]
    public string prefabJsonText;

    private Transform RegistryParent => PrefabRegistry.Instance.transform;

    [ContextMenu("Execute Prefab JSON")]
    public void ExecutePrefabJson()
    {
        ExecutePrefabJsonFromText(prefabJsonText);
    }

    public GameObject ExecutePrefabJsonFromText(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("PrefabJsonExecutor: JSON is empty");
            return null;
        }

        PrefabJson data;
        try
        {
            data = JsonUtility.FromJson<PrefabJson>(jsonText);
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON parse failed: {e}");
            return null;
        }

        if (data == null || data.steps == null)
        {
            Debug.LogError("Invalid PrefabJson structure");
            return null;
        }

       var root=  ExecuteCreateObject(data.steps);
        ExecuteAddChild(data.steps);
        ExecuteMove(data.steps);
        ExecuteAddComponent(data.steps);
        ExecuteSetProperty(data.steps);

        Debug.Log($"PrefabJsonExecutor: Build finished [{data.prefabName}]");
        return root;
    }

    #region Step Execution

    GameObject ExecuteCreateObject(List<PrefabStep> steps)
    {
        GameObject lastCreated = null;

        foreach (var s in steps)
        {
            if (s.type != "create_object") continue;

            if (PrefabRegistry.Instance.IsRegistered(s.id))
                continue;

            var go = new GameObject(s.name);
            // go.SetActive(false); // 保持 prefab 自身 Active 状态
            go.transform.SetParent(RegistryParent, false);

            // 注册
            PrefabRegistry.Instance.RegisterPrefabRoot(s.id, go);

            lastCreated = go;
        }

        return lastCreated;
    }


    void ExecuteAddChild(List<PrefabStep> steps)
    {
        foreach (var s in steps)
        {
            if (s.type != "add_child") continue;

            var parent = PrefabRegistry.Instance.GetObject(s.parent);
            if (parent == null)
            {
                Debug.LogWarning($"add_child parent not found: {s.parent}");
                continue;
            }

            GameObject child;

            var artPrefab = PrefabRegistry.Instance.GetArtPrefabByName(s.name);
            if (artPrefab != null)
            {
                child = Instantiate(artPrefab, parent.transform);
                child.name = artPrefab.name;
            }
            else
            {
                child = new GameObject(s.name);
                child.transform.SetParent(parent.transform, false);
            }

           // child.SetActive(false);

            // ✅ 直接使用 JSON 指定的 id 注册，不再拼 parentId
            PrefabRegistry.Instance.RegisterPrefabRoot(s.id, child);
        }
    }

    void ExecuteMove(List<PrefabStep> steps)
    {
        foreach (var s in steps)
        {
            if (s.type != "move") continue;

            var target = PrefabRegistry.Instance.FindObjectById(s.target);
            var parent = PrefabRegistry.Instance.FindObjectById(s.parent);

            if (target != null && parent != null)
                target.transform.SetParent(parent.transform, true);
            else
                Debug.LogWarning($"Move failed: target ({s.target}) or parent ({s.parent}) not found.");
        }
    }

    void ExecuteAddComponent(List<PrefabStep> steps)
    {
        foreach (var s in steps)
        {
            if (s.type != "add_component") continue;

            var targetObject = PrefabRegistry.Instance.FindObjectById(s.target);
            if (targetObject == null)
            {
                Debug.LogWarning($"add_component target not found: {s.target}");
                continue;
            }

            var type = ResolveType(s.component);
            if (type == null)
            {
                Debug.LogWarning($"Component type not found: {s.component} on {targetObject.name}");
                continue;
            }

            if (targetObject.GetComponent(type) == null)
                targetObject.AddComponent(type);
        }
    }

    void ExecuteSetProperty(List<PrefabStep> steps)
    {
        foreach (var s in steps)
        {
            if (s.type != "set_property") continue;

            var targetObject = PrefabRegistry.Instance.FindObjectById(s.target);
            if (targetObject == null)
            {
                Debug.LogWarning($"set_property target not found: {s.target}");
                continue;
            }

            var compType = ResolveType(s.component);
            if (compType == null) { Debug.LogWarning($"Component type not found: {s.component}"); continue; }

            var comp = targetObject.GetComponent(compType);
            if (comp == null) { Debug.LogWarning($"Component not found: {s.component}"); continue; }

            var field = compType.GetField(s.property);
            var prop = compType.GetProperty(s.property);

            if (field == null && prop == null)
            {
                Debug.LogWarning($"Property not found: {s.component}.{s.property} on {targetObject.name}");
                continue;
            }

            var targetType = field != null ? field.FieldType : prop.PropertyType;
            var valueObj = ParseValue(s.value, targetType);

            if (field != null) field.SetValue(comp, valueObj);
            else prop.SetValue(comp, valueObj);

            Debug.Log($"[set_property] {s.component}.{s.property} = {valueObj} on {targetObject.name}");
        }
    }

    #endregion

    #region Helpers

    Type ResolveType(string name)
    {
        var t = Type.GetType("UnityEngine." + name + ", UnityEngine");
        if (t != null) return t;
        return Type.GetType(name + ", Assembly-CSharp");
    }

    object ParseValue(string raw, Type targetType)
    {
        if (string.IsNullOrEmpty(raw)) return null;

        if (raw.StartsWith("object:"))
            return PrefabRegistry.Instance.GetObject(raw.Substring(7));

        if (raw.StartsWith("component:"))
        {
            var p = raw.Split(':');
            if (p.Length == 3 && PrefabRegistry.Instance.GetObject(p[1]) is GameObject go)
                return go.GetComponent(ResolveType(p[2]));
        }

        var parts = raw.Split(',');

        if (targetType == typeof(Vector3))
            return new Vector3(Parse(parts, 0), Parse(parts, 1), Parse(parts, 2));

        if (targetType == typeof(Vector2))
            return new Vector2(Parse(parts, 0), Parse(parts, 1));

        if (targetType == typeof(Quaternion))
            return new Quaternion(Parse(parts, 0), Parse(parts, 1), Parse(parts, 2), Parse(parts, 3));

        if (targetType == typeof(Color))
            return new Color(Parse(parts, 0), Parse(parts, 1), Parse(parts, 2), Parse(parts, 3));

        if (targetType == typeof(int)) return int.Parse(raw);
        if (targetType == typeof(float)) return float.Parse(raw);
        if (targetType == typeof(bool)) return bool.Parse(raw);
        if (targetType == typeof(string)) return raw;

        return null;
    }

    float Parse(string[] arr, int index)
    {
        if (arr.Length <= index) return 0f;
        float.TryParse(arr[index], out var v);
        return v;
    }

    #endregion
}

// JSON 数据结构
[Serializable]
public class PrefabJson
{
    public string prefabName;
    public List<PrefabStep> steps;
}

[Serializable]
public class PrefabStep
{
    public string type;      // create_object / add_child / move / add_component / set_property
    public string id;        // 对象 ID（直接使用，不拼 parentId）
    public string parent;    // 用于 add_child / move
    public string target;    // 用于 move / add_component / set_property
    public string name;      // 对象名字
    public string component; // 组件类型
    public string property;  // 属性名
    public string value;     // 属性值
}
