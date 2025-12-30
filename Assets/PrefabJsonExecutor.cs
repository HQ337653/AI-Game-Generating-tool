using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

#region JSON Structs

[Serializable]
public class PrefabStep
{
    public string type;        // create_object / add_child / move / add_component / set_property
    public string id;          // object id
    public string name;        // object name
    public string parent;      // parent id
    public string target;      // target object id
    public string component;   // component name
    public string property;    // property name
    public string value;       // value (string-encoded)
}

[Serializable]
public class PrefabRoot
{
    public string id;
    public string name;
}

[Serializable]
public class PrefabJson
{
    public string prefabName;
    public PrefabRoot root;
    public List<PrefabStep> steps;
}

#endregion

public class PrefabJsonExecutor : MonoBehaviour
{
    [TextArea(20, 40)]
    public string prefabJsonText;

    private Transform RegistryParent => PrefabRegistry.Instance.transform;

    [ContextMenu("Execute Prefab JSON")]
    public void ExecutePrefabJson()
    {
        ExecutePrefabJsonFromText(prefabJsonText);
    }

    public void ExecutePrefabJsonFromText(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("PrefabJsonExecutor: JSON is empty");
            return;
        }

        PrefabJson data;
        try
        {
            data = JsonUtility.FromJson<PrefabJson>(jsonText);
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON parse failed: {e}");
            return;
        }

        if (data == null || data.steps == null)
        {
            Debug.LogError("Invalid PrefabJson structure");
            return;
        }

        ExecuteCreateObject(data.steps);
        ExecuteAddChild(data.steps);
        ExecuteMove(data.steps);
        ExecuteAddComponent(data.steps);
        ExecuteSetProperty(data.steps);

        Debug.Log($"PrefabJsonExecutor: Build finished [{data.prefabName}]");
    }

    #region Step Execution

    void ExecuteCreateObject(List<PrefabStep> steps)
    {
        foreach (var s in steps)
        {
            if (s.type != "create_object") continue;

            // Check if the object is already registered
            if (PrefabRegistry.Instance.IsRegistered(s.id))
                continue;

            var go = new GameObject(s.name);
            go.SetActive(false);
            go.transform.SetParent(RegistryParent, false);

            // Register in PrefabRegistry
            PrefabRegistry.Instance.RegisterPrefabRoot(s.id, go);
        }
    }

    void ExecuteAddChild(List<PrefabStep> steps)
    {
        foreach (var s in steps)
        {
            if (s.type != "add_child") continue;

            if (!PrefabRegistry.Instance.IsRegistered(s.parent))
            {
                Debug.LogWarning($"add_child parent not found: {s.parent}. Trying to add child {s.name} to parent {s.parent}");
                continue;
            }

            var parent = PrefabRegistry.Instance.GetObject(s.parent);
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

            child.SetActive(false);
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
            {
                target.transform.SetParent(parent.transform, true);
            }
            else
            {
                Debug.LogWarning($"Move failed: target ({s.target}) or parent ({s.parent}) not found. Target: {target}, Parent: {parent}");
            }
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
                Debug.LogWarning($"add_component target not found: {s.target}. Target object: {targetObject}");
                continue;
            }

            var type = ResolveType(s.component);
            if (type == null)
            {
                Debug.LogWarning($"Component type not found: {s.component}. Trying to add component to {targetObject.name}");
                continue;
            }

            // Add the component only if it doesn't exist
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
                Debug.LogWarning($"set_property target not found: {s.target}. Target object: {targetObject}");
                continue;
            }

            var compType = ResolveType(s.component);
            if (compType == null)
            {
                
                Debug.LogWarning($"Component type not found for {s.component} on object {targetObject.name}");
                continue;
            }

            var comp = targetObject.GetComponent(compType);
            if (comp == null)
            {
                Debug.LogWarning($"Component not found: {s.component} on {targetObject.name}. Trying to set property: {s.property}");
                continue;
            }

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

        // 最后统一激活所有注册的对象
    }

    #endregion

    #region Helpers

    Type ResolveType(string name)
    {
        var t = Type.GetType("UnityEngine." + name + ", UnityEngine");
        if (t != null) return t;
        return Type.GetType("Assembly-CSharp." + name + ", Assembly-CSharp");
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
