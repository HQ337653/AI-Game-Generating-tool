
using System.Collections.Generic;
using UnityEngine;
using static AiHelper.UserResource;
namespace AiHelper
{
    [CreateAssetMenu(fileName = "UserResource", menuName = "Resources/UserResource", order = 2)]
    public class UserResource : ScriptableObject
    {
        public string ProjectName;
        public bool is3D;
        public int currentStageIndex;
        public string GameDesign;
        public string SystemDesign;
        public string GameStagesDescription;
        public List<TitleAndDescription> gameDevelopementStages;
        public string sceneDesignDescription;

        public GameStageInfo CurrentStageInfo { get { if (GameStageInfos!=null&& currentStageIndex< GameStageInfos.Count) { return GameStageInfos[currentStageIndex];} return null; } }
        public List<GameStageInfo> GameStageInfos;
        [ContextMenu("showCode")]
        void logCurrentCodeInfo()
        {
            Debug.Log(currentCodeInfo());
        }
        public string currentCodeInfo()
        {
            return readFileUtility.GenerateAllCodeDescription(ResourceData.ProjectScriptLocation(ProjectName)).Item1.ToString();
        }
        public List<TitleAndDescription> makeGameDevelopmentStageByJson(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText))
            {
                Debug.LogError("JSON text is null or empty.");
                return new List<TitleAndDescription>();
            }

            StageListWrapper wrapper = JsonUtility.FromJson<StageListWrapper>(jsonText);

            if (wrapper == null || wrapper.stages == null)
            {
                Debug.LogError("Failed to parse JSON into stages.");
                return new List<TitleAndDescription>();
            }

            // 保存解析的阶段描述
            gameDevelopementStages = wrapper.stages;

            // 初始化 GameStageInfos，确保数量与阶段数一致
            if (GameStageInfos == null)
                GameStageInfos = new List<GameStageInfo>();
            else
                GameStageInfos.Clear();

            foreach (var stage in gameDevelopementStages)
            {
                GameStageInfos.Add(new GameStageInfo());
            }

            return gameDevelopementStages;
        }


        [System.Serializable]
        public class StageListWrapper
        {
            public List<TitleAndDescription> stages;
        }
        [System.Serializable]
        public class ScriptBatchesWrapper
        {
            public List<CodeBatch> batches;
        }
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
    }
    [System.Serializable]
    public class GameStageInfo
    {
        public string CodeDesign = "";
        public string CurrentStageAllPrefabDescription = "";
        public string MakeSceneDescription = "";
        public string ScenePrefabSingletonDescription = "";
        public List<CodeBatch> CodeBatchs = new List<CodeBatch>();
        public List<TitleAndDescription> ScenePrefabSingletonStages = new List<TitleAndDescription>();
        public List<TitleAndDescription> PrefabDesign = new List<TitleAndDescription>();
        public int currentCodeBatchIndex = 0;
        public List<TitleAndDescription> MakeSceneStages = new List<TitleAndDescription>();
        public int currentPrefabIndex;
        public int currentSceneIndex;
        public string allPrefabCommand;

        public List<TitleAndDescription> makePrefabDesignByJson(string jsonText)
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
            return PrefabDesign;
        }

        public List<TitleAndDescription> makeScenePrefabSingletonByJson(string jsonText)
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

            ScenePrefabSingletonStages = wrapper.stages;
            return ScenePrefabSingletonStages;
        }
        public List<TitleAndDescription> makeSceneDesignByJson(string jsonText)
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
            Debug.Log(wrapper.stages.Count);
            MakeSceneStages = wrapper.stages;
            return MakeSceneStages;
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



    }

    [System.Serializable]
    public class TitleAndDescription
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
}