using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace AiHelper
{
    [CreateAssetMenu(fileName = "UserResource", menuName = "Resources/UserResource", order = 2)]
    public class UserResource : ScriptableObject
    {
        private static UserResource _instance;

        // 公开属性，获取唯一实例
        public static UserResource Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 尝试从资源中加载UserResource实例
                    _instance = Resources.Load<UserResource>("UserData");
                    if (_instance == null)
                    {
                        Debug.LogError("UserResource instance not found. Make sure it's placed under a Resources folder.");
                    }
                }
                return _instance;
            }
        }

        public string allPrefabDescription;

        [Header("Resource Metadata")]
        public string resourceName;  // 资源名称
        public string description;   // 简介
        public string resourcePath;  // 资源路径

        [Header("Resource File")]
        public TextAsset scriptFile; // 如果是脚本资源，可以存储文件

        public string GameDesign;

        public string SystemDesign;

        public string GameStagesDescription;

        public string CodeDesign;

        public string currentCodeInfo()
        {
           return readFileUtility.GenerateAllCodeDescription(ResourceData.ProjectScriptLocation(ProjectName)).Item1.ToString();
        }

        public string PrefabDesignDescription;

        public string ProjectName;

        public int currentStageIndex;
        // 用于展示的大周期 JSON 数据
        public List<Stage> stages;
        public List<Stage> PrefabDesign;
        public List<CodeBatch> CodeBatchs;
        public int currentCodeBatchIndex;

        public List<Stage> makeGameDevelopmentStageByJson(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                Debug.LogError("JSON text is null or empty.");
                return null;
            }

            StageListWrapper wrapper = JsonUtility.FromJson<StageListWrapper>(jsonText);

            if (wrapper == null || wrapper.stages == null)
            {
                Debug.LogError("Failed to parse JSON into stages.");
                return null;
            }

            stages = wrapper.stages;
            return stages;
        }
        

        public List<Stage> makePrefabDesignByJson(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                Debug.LogError("JSON text is null or empty.");
                return null;
            }

            StageListWrapper wrapper = JsonUtility.FromJson<StageListWrapper>(jsonText);

            if (wrapper == null || wrapper.stages == null)
            {
                Debug.LogError("Failed to parse JSON into stages.");
                return null;
            }

            PrefabDesign = wrapper.stages;
            return stages;
        }
        public List<CodeBatch> makeCodeGenerationBatchByJson(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                Debug.LogError("JSON text is null or empty.");
                return null;
            }

            ScriptBatchesWrapper wrapper = JsonUtility.FromJson<ScriptBatchesWrapper>(jsonText);

            if (wrapper == null || wrapper.batches == null)
            {
                Debug.LogError("Failed to parse JSON into stages.");
                return null;
            }

            CodeBatchs = wrapper.batches;
            return CodeBatchs;
        }

        internal string currentArt()
        {
            return readFileUtility.GetAllFileNames("Assets/Art");
        }

        [System.Serializable]
        public class StageListWrapper
        {
            public List<Stage> stages;
        }
        [System.Serializable]
        public class ScriptBatchesWrapper
        {
            public List<CodeBatch> batches;
        }
    }


    public enum StageStatus
    {
        Inactive, // 未激活
        Current,  // 当前阶段
        Active    // 激活阶段
    }

    [System.Serializable]
    public class Stage
    {
        public string title;
        public string description;
    }

    [System.Serializable]
    public class CodeBatch
    {
        public int batchIndex;
        public List<string> scripts;
    }



    public class GameCycle : ScriptableObject
    {
        public Stage[] stages; // 所有阶段的链表
    }
}