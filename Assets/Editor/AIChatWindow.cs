namespace AiHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using static AiHelper.MainWindow;

    public class DebugInputData
    {
        
        public string label;
        public string prompt;
        public DebugWindow.DebugActionType actionType;
        public Action<string, DebugWindow.DebugActionType> onInputReceived;
    }

    public class DebugWindow : EditorWindow
    {
        private string debugInput = "";
        private string displayPrompt = "";
        private DebugActionType actionType;
        private string windowLabel = "Debug Window";

        // 当前窗口的数据
        private DebugInputData currentInputData;

        public enum DebugActionType
        {
            PlayDesign,
            ModifyLess,
            ModifyMore,
            Discuss,
            DevelopPlanConfirm,
            SystemDesignConfirm,
            CodeDesignModify,
            CodeConfirm,
            ExecuteCodeBatch
        }

        public static void OpenDebugWindow(DebugInputData inputData)
        {
            
            DebugWindow window = GetWindow<DebugWindow>("Debug Window");
            window.Show();
            window.displayPrompt = inputData.prompt;
            window.actionType = inputData.actionType;
            window.windowLabel = inputData.label;
            window.currentInputData = inputData;
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"{windowLabel}", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.Label("Prompt:", EditorStyles.boldLabel);
            GUILayout.TextArea(displayPrompt, GUILayout.Height(100));

            if (GUILayout.Button("复制 Prompt 到剪贴板"))
            {
                GUIUtility.systemCopyBuffer = displayPrompt;
                Debug.Log("[DebugWindow] Prompt copied to clipboard!");
            }

            GUILayout.Space(20);

            GUILayout.Label("AI Response:", EditorStyles.boldLabel);
            debugInput = EditorGUILayout.TextArea(debugInput, GUILayout.Height(150));

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("取消", GUILayout.Width(100)))
            {
                this.Close();
            }

            if (GUILayout.Button("确认", GUILayout.Width(100)))
            {
                if (string.IsNullOrEmpty(debugInput))
                {
                    EditorUtility.DisplayDialog("错误", "请先输入AI的回复", "确定");
                    return;
                }

                // 触发回调
                currentInputData?.onInputReceived?.Invoke(debugInput, actionType);
                this.Close();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }


    public class MainWindow : EditorWindow
    {
        // =========================
        // 基础状态
        // =========================
        private int currentPage = 0;
        private string[] pageNames = new string[]
        {
        "大周期展示",
        "代码生成",
        "Prefab",
        "搭建场景",
        "Debug验收"
        };

        // =========================
        // 代码生成相关状态
        // =========================
        private Vector2 codeScrollPosition = Vector2.zero;
        private List<string> pendingCodeCommands = new List<string>();

        // =========================
        // Prefab相关状态
        // =========================
        private Vector2 prefabScrollPosition = Vector2.zero;
        private int currentPrefabIndex = 0;

        // =========================
        // 窗口
        // =========================
        [MenuItem("Window/Main Window")]
        public static void ShowWindow()
        {
            GetWindow<MainWindow>("Main Window");
        }

        private void OnGUI()
        {
            GUILayout.Label("Main Window", EditorStyles.boldLabel);

            switch (currentPage)
            {
                case 0:
                    ShowCyclePage();
                    break;
                case 1:
                    ShowCodeGenerationPage();
                    break;
                case 2:
                    ShowPrefabPage();
                    break;
                case 3:
                    ShowSceneBuildingPage();
                    break;
                case 4:
                    ShowDebugAcceptancePage();
                    break;
            }
        }

        // =========================
        // Page 0：大周期展示
        // =========================
        private void ShowCyclePage()
        {
            GUILayout.Label("大周期展示", EditorStyles.boldLabel);

            var stages = UserResource.Instance.stages;
            int currentStageIndex = UserResource.Instance.currentStageIndex;

            GUILayout.BeginVertical();

            if (stages == null || stages.Count == 0)
            {
                if (GUILayout.Button("生成开发计划"))
                {
                    AIChatWindow.OpenAIChatWindow(AIChatWindow.chatWindowStages.DevelopmentPlan);
                }
            }
            else
            {
                for (int i = 0; i < stages.Count; i++)
                {
                    Color origin = GUI.color;

                    if (i == currentStageIndex)
                        GUI.color = Color.green;
                    else if (i < currentStageIndex)
                        GUI.color = Color.gray;

                    GUILayout.Label(stages[i].title, EditorStyles.boldLabel);
                    GUILayout.Label(stages[i].description);

                    GUI.color = origin;
                    GUILayout.Space(10);
                }
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("下一步") && currentPage < pageNames.Length - 1)
            {
                currentPage++;
            }

            GUILayout.EndVertical();
        }

        // =========================
        // Page 1：代码生成
        // =========================
        private void ShowCodeGenerationPage()
        {
            GUILayout.Label("代码生成", EditorStyles.boldLabel);

            var batches = UserResource.Instance.CodeBatchs;
            int currentBatch = UserResource.Instance.currentCodeBatchIndex;

            if (batches == null || batches.Count == 0)
            {
                GUILayout.Label("还没有生成代码计划");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("进入对话窗口"))
                {
                    AIChatWindow.OpenAIChatWindow(AIChatWindow.chatWindowStages.MakeCodePlan);
                }
                return;
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);
            codeScrollPosition = GUILayout.BeginScrollView(codeScrollPosition, GUILayout.Height(position.height - 140));

            for (int i = 0; i < batches.Count; i++)
            {
                var batch = batches[i];
                bool isFinished = i < currentBatch;
                bool isCurrent = i == currentBatch;

                Color origin = GUI.color;
                GUI.color = isFinished ? Color.gray : Color.white;

                foreach (var script in batch.scripts)
                {
                    if (isCurrent)
                        GUILayout.Label(script, EditorStyles.boldLabel);
                    else
                        GUILayout.Label(script);
                }

                GUI.color = origin;
                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool hasMoreBatch = currentBatch < batches.Count;

            if (hasMoreBatch)
            {
                if (GUILayout.Button("继续生成", GUILayout.Width(140), GUILayout.Height(40)))
                {
                    var scripts = batches[currentBatch].scripts;

                    string prompt = ResourceData.WriteCodeCommandFromScriptName(
                        UserResource.Instance.CodeDesign,
                        UserResource.Instance.SystemDesign,
                        scripts,
                        UserResource.Instance.ProjectName
                    );

                    var inputData = new DebugInputData
                    {
                        label = "执行代码生成（仅缓存）",
                        prompt = prompt,
                        actionType = DebugWindow.DebugActionType.ExecuteCodeBatch,
                        onInputReceived = ReceiveCodeBatchInput
                    };

                    DebugWindow.OpenDebugWindow(inputData);
                }
            }
            else
            {
                if (GUILayout.Button("下一步", GUILayout.Width(140), GUILayout.Height(40)))
                {
                    currentPage++;
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void ReceiveCodeBatchInput(string input, DebugWindow.DebugActionType actionType)
        {
            pendingCodeCommands.Add(input);
            UserResource.Instance.currentCodeBatchIndex++;
            Debug.Log($"已缓存代码 Batch：{UserResource.Instance.currentCodeBatchIndex}");
        }

        // =========================
        // Page 2：Prefab
        // =========================
        private void ShowPrefabPage()
        {
            GUILayout.Label("Prefab 设计", EditorStyles.boldLabel);
            GUILayout.Space(10);

            var prefabDesigns = UserResource.Instance.PrefabDesign;

            if (prefabDesigns == null || prefabDesigns.Count == 0)
            {
                GUILayout.Label("尚未生成 Prefab 设计。");

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("进入 Prefab 设计", GUILayout.Height(40)))
                {
                    // 打开 AIChatWindow 而不是直接 DebugWindow
                    AIChatWindow.OpenPrefabChatWindow();
                }

                return;
            }

            GUILayout.Label("Prefab 列表", EditorStyles.boldLabel);
            GUILayout.Space(5);

            prefabScrollPosition = GUILayout.BeginScrollView(prefabScrollPosition, GUILayout.Height(position.height - 180));
            for (int i = 0; i < prefabDesigns.Count; i++)
            {
                var prefab = prefabDesigns[i];

                GUI.color = i < currentPrefabIndex ? Color.gray : Color.white;

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label(prefab.title, EditorStyles.boldLabel);
                GUILayout.Label(prefab.description, EditorStyles.wordWrappedLabel);
                GUILayout.EndVertical();

                GUI.color = Color.white;
                GUILayout.Space(6);
            }
            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (currentPrefabIndex < prefabDesigns.Count)
            {
                if (GUILayout.Button("生成下一个 Prefab", GUILayout.Width(200), GUILayout.Height(40)))
                {
                    var prefab = prefabDesigns[currentPrefabIndex];
                    string prompt = ResourceData.MakeSinglePrefabJson(prefab.title,UserResource.Instance.allPrefabDescription, UserResource.Instance.currentCodeInfo(),  UserResource.Instance.currentArt());

                    var inputData = new DebugInputData
                    {
                        label = $"生成 Prefab：{prefab.title}",
                        prompt = prompt,
                        actionType = DebugWindow.DebugActionType.ExecuteCodeBatch,
                        onInputReceived = OnReceivePrefabGenerateJson
                    };

                    DebugWindow.OpenDebugWindow(inputData);
                }
            }
            else
            {
                if (GUILayout.Button("下一步", GUILayout.Width(140), GUILayout.Height(40)))
                {
                    currentPage++;
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public void OnReceivePrefabJson(string json, DebugWindow.DebugActionType actionType)
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("Prefab JSON 为空。");
                return;
            }

            var list = UserResource.Instance.makePrefabDesignByJson(json);
            if (list != null)
            {
                currentPrefabIndex = 0;
                Debug.Log($"Prefab Design JSON 解析成功，数量：{list.Count}");
                Repaint();
            }
            else
            {
                Debug.LogError("Prefab Design JSON 解析失败。");
            }
        }

        private void OnReceivePrefabGenerateJson(string json, DebugWindow.DebugActionType actionType)
        {
            Debug.Log($"Received Prefab JSON for index {currentPrefabIndex}");
            currentPrefabIndex++;
        }


        // =========================
        // Page 3：搭建场景
        // =========================

        private void ShowSceneBuildingPage()
        {
            GUILayout.Label("搭建场景", EditorStyles.boldLabel);
            GUILayout.Label("任务描述：创建并布局场景");

            if (GUILayout.Button("进入对话窗口"))
            {
                // TODO
            }
        }

        // =========================
        // Page 4：Debug验收
        // =========================

        private void ShowDebugAcceptancePage()
        {
            GUILayout.Label("Debug验收", EditorStyles.boldLabel);
            GUILayout.Label("任务描述：修复 bug，优化性能");

            if (GUILayout.Button("进入对话窗口"))
            {
                // TODO
            }
        }

        // =========================
        // Enum
        // =========================

        public enum chatWindowStages
        {
            DevelopmentPlan,
            MakeCodePlan,
            MakePrefabPlan
        }
    }


    public class AIChatWindow : EditorWindow
    {
        // ======================================
        // 基础状态
        // ======================================
        private string userInput = "";
        private string aiResponse = "";
        private string discussionInput = "";
        private chatWindowStages currentStage;
        private DevelopmentPlanStage developmentPlanStage;
        private Vector2 scrollPosition = Vector2.zero;
        private Rect lastWindowRect = Rect.zero;
        private string currentPrefabPrompt = "";
        private bool isPrefabMode = false;

        // ======================================
        // 枚举
        // ======================================
        public enum chatWindowStages { DevelopmentPlan, MakeCodePlan }
        public enum DevelopmentPlanStage { gameDesign, systemDesign, stageDesign }

        // ======================================
        // 打开 DevelopmentPlan 或 MakeCodePlan
        // ======================================
        public static void OpenAIChatWindow(chatWindowStages targetStage)
        {
            AIChatWindow window = GetWindow<AIChatWindow>("AI Chat Window");
            window.Show();
            window.currentStage = targetStage;
            window.position = new Rect(100, 100, 600, 700);

            if (targetStage == chatWindowStages.DevelopmentPlan)
                window.CheckPlayDesignStatus();
            else if (targetStage == chatWindowStages.MakeCodePlan)
                window.OpenCodeDesignDebug();
        }

        private void CheckPlayDesignStatus()
        {
            if (!string.IsNullOrEmpty(UserResource.Instance.GameDesign))
            {
                aiResponse = UserResource.Instance.GameDesign;
                developmentPlanStage = DevelopmentPlanStage.stageDesign;
            }
            else
            {
                aiResponse = "";
                developmentPlanStage = DevelopmentPlanStage.gameDesign;
            }
        }

        private void OpenCodeDesignDebug()
        {
            var inputData = new DebugInputData
            {
                label = "代码设计",
                prompt = ResourceData.MakeCodeDesignPrompt(
                    UserResource.Instance.GameDesign,
                    UserResource.Instance.SystemDesign,
                    UserResource.Instance.GameStagesDescription,
                    UserResource.Instance.stages[UserResource.Instance.currentStageIndex].description
                ),
                actionType = DebugWindow.DebugActionType.CodeDesignModify,
                onInputReceived = (input, actionType) => this.ReceiveAiResponse(input, actionType)
            };
            DebugWindow.OpenDebugWindow(inputData);
        }

        // ======================================
        // 打开 Prefab Chat
        // ======================================
        
        public static void OpenPrefabChatWindow()
        {
            AIChatWindow window = GetWindow<AIChatWindow>("Prefab 设计");
            window.Show();
            window.isPrefabMode = true;

            window.currentPrefabPrompt = ResourceData.MakePrefabDesign(
                UserResource.Instance.GameDesign,
                UserResource.Instance.SystemDesign,
                UserResource.Instance.currentCodeInfo()
            );
        }

        // ======================================
        // GUI
        // ======================================
        private void OnGUI()
        {
            if (position != lastWindowRect)
            {
                lastWindowRect = position;
                Repaint();
            }

            if (isPrefabMode)
                RenderPrefabChat();
            else
            {
                switch (currentStage)
                {
                    case chatWindowStages.DevelopmentPlan:
                        RenderDevelopmentPlanStage();
                        break;
                    case chatWindowStages.MakeCodePlan:
                        RenderMakeCodePlanStage();
                        break;
                    default:
                        RenderNotImplementedStage();
                        break;
                }
            }
        }

        // ======================================
        // Prefab Chat
        // ======================================
        private void RenderPrefabChat()
        {
            GUILayout.Label("Prefab 设计讨论", EditorStyles.boldLabel);

            GUILayout.Label("AI 提示:");
            GUILayout.TextArea(currentPrefabPrompt, GUILayout.Height(100));

            GUILayout.Label("讨论内容:");
            discussionInput = EditorGUILayout.TextArea(discussionInput, GUILayout.Height(150));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("确认设计"))
            {
                // 调用 DebugWindow 生成 JSON
                var inputData = new DebugInputData
                {
                    label = "生成 Prefab JSON",
                    prompt = ResourceData.MakeJsonForAllPrefab(discussionInput),
                    actionType = DebugWindow.DebugActionType.CodeConfirm,
                    onInputReceived = MainWindow.GetWindow<MainWindow>().OnReceivePrefabJson
                };
                DebugWindow.OpenDebugWindow(inputData);
                this.Close();
            }

            if (GUILayout.Button("取消"))
            {
                this.Close();
            }
            GUILayout.EndHorizontal();
        }

        // ======================================
        // DevelopmentPlan
        // ======================================
        private void RenderDevelopmentPlanStage()
        {
            switch (developmentPlanStage)
            {
                case DevelopmentPlanStage.gameDesign:
                    RenderGameDesignPhase();
                    break;
                case DevelopmentPlanStage.systemDesign:
                    RenderSystemDesignPhase();
                    break;
                case DevelopmentPlanStage.stageDesign:
                    RenderStageDesignPhase();
                    break;
            }
        }

        private void RenderGameDesignPhase()
        {
            GUILayout.Label("玩法设计", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            GUILayout.Label("AI Response: 暂时留空", EditorStyles.wordWrappedLabel);
            GUILayout.EndScrollView();

            GUILayout.Label("请输入玩法设计");
            userInput = EditorGUILayout.TextArea(userInput, GUILayout.Height(100));

            if (GUILayout.Button("发送"))
            {
                UserResource.Instance.GameDesign = userInput;
                var inputData = new DebugInputData
                {
                    label = "玩法设计",
                    prompt = ResourceData.MakeSystemDescriptionPrompt(userInput),
                    actionType = DebugWindow.DebugActionType.PlayDesign,
                    onInputReceived = (input, actionType) => this.ReceiveAiResponse(input, actionType)
                };
                DebugWindow.OpenDebugWindow(inputData);
            }
        }

        private void RenderSystemDesignPhase()
        {
            GUILayout.Label("系统设计", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            GUILayout.Label(UserResource.Instance.SystemDesign, EditorStyles.wordWrappedLabel);
            GUILayout.EndScrollView();

            if (GUILayout.Button("确认"))
            {
                var inputData = new DebugInputData
                {
                    label = "确认系统设计",
                    prompt = ResourceData.MakedevelopmentPlanDescription(UserResource.Instance.SystemDesign, UserResource.Instance.GameDesign),
                    actionType = DebugWindow.DebugActionType.SystemDesignConfirm,
                    onInputReceived = (input, actionType) => this.ReceiveAiResponse(input, actionType)
                };
                DebugWindow.OpenDebugWindow(inputData);
            }
        }

        private void RenderStageDesignPhase()
        {
            GUILayout.Label("开发阶段", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            GUILayout.Label(aiResponse, EditorStyles.wordWrappedLabel);
            GUILayout.EndScrollView();

            discussionInput = EditorGUILayout.TextArea(discussionInput, GUILayout.Height(100));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("发送讨论"))
            {
                var inputData = new DebugInputData
                {
                    label = "讨论",
                    prompt = discussionInput,
                    actionType = DebugWindow.DebugActionType.Discuss,
                    onInputReceived = (input, actionType) => this.ReceiveAiResponse(input, actionType)
                };
                DebugWindow.OpenDebugWindow(inputData);
                discussionInput = "";
            }

            if (GUILayout.Button("确认阶段"))
            {
                var inputData = new DebugInputData
                {
                    label = "生成阶段JSON",
                    prompt = ResourceData.DevelopePlanMakeJson(aiResponse),
                    actionType = DebugWindow.DebugActionType.DevelopPlanConfirm,
                    onInputReceived = (input, actionType) => this.ReceiveAiResponse(input, actionType)
                };
                DebugWindow.OpenDebugWindow(inputData);
            }
            GUILayout.EndHorizontal();
        }

        // ======================================
        // MakeCodePlan
        // ======================================
        private void RenderMakeCodePlanStage()
        {
            GUILayout.Label("代码设计", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            GUILayout.Label(aiResponse, EditorStyles.wordWrappedLabel);
            GUILayout.EndScrollView();

            string codeUserInput = EditorGUILayout.TextArea("", GUILayout.Height(100));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("发送修改"))
            {
                var inputData = new DebugInputData
                {
                    label = "代码设计修改",
                    prompt = ResourceData.CodeDesignModificationPrompt(codeUserInput, aiResponse,
                        UserResource.Instance.GameDesign,
                        UserResource.Instance.SystemDesign,
                        UserResource.Instance.GameStagesDescription,
                        UserResource.Instance.stages[UserResource.Instance.currentStageIndex].description),
                    actionType = DebugWindow.DebugActionType.CodeDesignModify,
                    onInputReceived = (input, actionType) => this.ReceiveAiResponse(input, actionType)
                };
                DebugWindow.OpenDebugWindow(inputData);
            }

            if (GUILayout.Button("确认代码"))
            {
                var inputData = new DebugInputData
                {
                    label = "确认代码",
                    prompt = ResourceData.CodeBatchMakeJson(aiResponse),
                    actionType = DebugWindow.DebugActionType.CodeConfirm,
                    onInputReceived = (input, actionType) => this.ReceiveAiResponse(input, actionType)
                };
                DebugWindow.OpenDebugWindow(inputData);
            }

            GUILayout.EndHorizontal();
        }

        private void RenderNotImplementedStage()
        {
            GUILayout.Label("功能尚未实现", EditorStyles.boldLabel);
        }

        // ======================================
        // 回调处理
        // ======================================
        public void ReceiveAiResponse(string content, DebugWindow.DebugActionType actionType)
        {
            switch (actionType)
            {
                case DebugWindow.DebugActionType.PlayDesign:
                    UserResource.Instance.SystemDesign = content;
                    developmentPlanStage = DevelopmentPlanStage.systemDesign;
                    break;
                case DebugWindow.DebugActionType.SystemDesignConfirm:
                    aiResponse = content;
                    developmentPlanStage = DevelopmentPlanStage.stageDesign;
                    break;
                case DebugWindow.DebugActionType.DevelopPlanConfirm:
                    aiResponse = content;
                    UserResource.Instance.makeGameDevelopmentStageByJson(content);
                    break;
                case DebugWindow.DebugActionType.CodeConfirm:
                    UserResource.Instance.CodeDesign = aiResponse;
                    UserResource.Instance.makeCodeGenerationBatchByJson(content);
                    this.Close();
                    break;
                case DebugWindow.DebugActionType.CodeDesignModify:
                    UserResource.Instance.CodeDesign = content;
                    aiResponse = content;
                    break;
                case DebugWindow.DebugActionType.ModifyLess:
                case DebugWindow.DebugActionType.ModifyMore:
                case DebugWindow.DebugActionType.Discuss:
                    aiResponse = content;
                    break;
            }
            Repaint();
        }
    }
}