using AiHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static MainWin;

#region Test Menu Entry

public static class TestWindowLauncher
{
    [MenuItem("Window/AI游戏制作助手")]
    public static void OpenAllTestWindows()
    {
        MainWin.Open();
        //requestAiResponse.Open();
        //AIChatWindow.Open();
    }
}

#endregion


#region Main Window

public class MainWin : EditorWindow
{
    private MainPage currentPage = MainPage.DevelopmentPlan;

    public static void Open()
    {
        MainWin window = GetWindow<MainWin>("Main Window");
        window.position = new Rect(50, 100, 500, 600);
        window.Show();
    }

    public void GoToPage(MainPage page)
    {
        currentPage = page;
        Repaint();
    }

    private void OnGUI()
    {
        GUILayout.Label("Main Window", EditorStyles.boldLabel);
        GUILayout.Space(10);

        switch (currentPage)
        {
            case MainPage.DevelopmentPlan:
                DevelopmentPlanView.Draw(this);
                break;

            case MainPage.Code:
                CodeGenerationView.Draw(this);
                break;
            case MainPage.PrefabSceneSingleton:
                ScenePrefabSingleton.Draw(this);
                break;
            case MainPage.Prefab:
                PrefabGenerationView.Draw(this);
                break;

            case MainPage.SceneBuilding:
                SceneBuildingView.Draw(this);
                break;

            case MainPage.DebugAcceptance:
                DebugAcceptanceView.Draw(this);
                break;
        }
    }

    public enum MainPage
    {
        DevelopmentPlan = 0,
        Code = 1,
        PrefabSceneSingleton = 2,
        Prefab = 3,
        SceneBuilding = 4,
        DebugAcceptance = 5
    }
}



public class AIChatWindow : EditorWindow
{
    // ===== 状态 =====
    public string aiResponse = "";
    public string InputTextLable = "Input";
    private string userInput = "";
    private Vector2 scrollPosition;

    private string windowLabel = "AI Chat";

    // ===== 按钮显示控制 =====
    public bool showSendButton = true;
    public bool showConfirmButton = true;

    // ===== 事件回调 =====
    public event Action<string> OnSubmit;
    public event Action<string> OnConfirm;

    // ===== 打开窗口，返回实例 =====
    public static AIChatWindow Open(string label = "AI Chat")
    {
        AIChatWindow window = GetWindow<AIChatWindow>(label);
        window.windowLabel = label;
        window.position = new Rect(580, 100, 600, 500);
        window.Show();
        return window;
    }

    /// <summary>
    /// 设置窗口状态：AI Response、输入框标签、按钮显示
    /// </summary>
    public void SetState(string aiResponseContent, string inputLabel, bool sendBtnVisible, bool confirmBtnVisible)
    {
        aiResponse = aiResponseContent;
        InputTextLable = inputLabel;
        showSendButton = sendBtnVisible;
        showConfirmButton = confirmBtnVisible;
        userInput = "";
    }

    /// <summary>
    /// 清空用户输入
    /// </summary>
    public void ClearInput()
    {
        userInput = "";
    }

    private void OnGUI()
    {
        GUILayout.Label(windowLabel, EditorStyles.boldLabel);
        GUILayout.Space(5);

        // ===== AI Response 区域 =====
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height - 260));
        EditorGUILayout.TextArea(aiResponse, EditorStyles.wordWrappedLabel, GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();
        GUILayout.FlexibleSpace();

        // ===== 输入区域 =====
        GUILayout.Label(InputTextLable, EditorStyles.boldLabel);
        userInput = EditorGUILayout.TextArea(userInput, GUILayout.Height(80));
        GUILayout.Space(10);

        // ===== 按钮区域 =====
        if (showSendButton)
        {
            if (GUILayout.Button("发送"))
            {
                OnSubmit?.Invoke(userInput);
            }
        }

        if (showConfirmButton)
        {
            if (GUILayout.Button("不用修改了，就这样"))
            {
                OnConfirm?.Invoke(userInput);
            }
        }
    }
}



#endregion


#region Request AI Response Window
public class requestAiResponse : EditorWindow
{
    private string aiResponse = "";
    private string displayPrompt = "";
    private string windowLabel = "Request AI Response";

    // ===== 实例事件，代替 static =====
    public event Action<string> OnConfirmed;

    // ===== 打开窗口，带 prompt 参数 =====
    public static requestAiResponse Open(string prompt = "")
    {
        var window = GetWindow<requestAiResponse>("Request AI Response");
        window.displayPrompt = prompt;
        window.windowLabel = "Request AI Response";
        window.position = new Rect(200, 650, 600, 520);
        window.Show();
        return window;
    }

    public void SendPrompt(string prompt)
    {
        displayPrompt = prompt;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Label(windowLabel, EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Prompt 显示
        GUILayout.Label("Prompt:", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(displayPrompt, GUILayout.Height(100));

        if (GUILayout.Button("复制 Prompt 到剪贴板"))
        {
            GUIUtility.systemCopyBuffer = displayPrompt;
        }

        GUILayout.FlexibleSpace();

        // AI Response 输入
        GUILayout.Label("AI Response:", EditorStyles.boldLabel);
        aiResponse = EditorGUILayout.TextArea(aiResponse, GUILayout.Height(150));

        GUILayout.Space(20);

        // 按钮
        if (GUILayout.Button("取消"))
        {
            Close();
        }

        if (GUILayout.Button("确认"))
        {
            OnConfirmed?.Invoke(aiResponse);
            Close();
        }

        GUILayout.EndVertical();
    }
}




#endregion



public class DevelopmentPlanView
{
    private static Vector2 scrollPosition;

    // ===== 项目初始化临时状态 =====
    private static string tempProjectName = "";
    private static bool tempIs3D = true;

    public static void Draw(MainWin win)
    {
        // =========================
        // Step 0：项目初始化页
        // =========================
        if (string.IsNullOrEmpty(UserResource.Instance.ProjectName))
        {
            DrawProjectInitPage(win);
            return;
        }

        // =========================
        // 原有页面：大周期展示
        // =========================
        GUILayout.Label("大周期展示", EditorStyles.boldLabel);
        GUILayout.Space(10);

        var stages = UserResource.Instance.gameDevelopementStages;
        int currentStageIndex = UserResource.Instance.currentStageIndex;

        scrollPosition = GUILayout.BeginScrollView(
            scrollPosition,
            GUILayout.ExpandHeight(true)
        );

        if (stages == null || stages.Count == 0)
        {
            GUILayout.Label("尚未生成开发计划。");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("生成开发计划", GUILayout.Height(40)))
            {
                OpenGameDesignChat();
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

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label(stages[i].title, EditorStyles.boldLabel);
                GUILayout.Label(stages[i].description, EditorStyles.wordWrappedLabel);
                GUILayout.EndVertical();

                GUI.color = origin;
                GUILayout.Space(8);
            }

            GUILayout.Space(20);
            if (GUILayout.Button("下一页", GUILayout.Height(40)))
            {
                win.GoToPage(MainPage.Code);
            }
        }

        GUILayout.EndScrollView();
    }

    // =========================
    // 项目初始化页面
    // =========================
    private static void DrawProjectInitPage(MainWin win)
    {
        GUILayout.Label("项目初始化", EditorStyles.boldLabel);
        GUILayout.Space(15);

        GUILayout.Label("项目名称（请输入英文，不要包含特殊字符）");

        // 过滤非法字符：只保留字母、数字、下划线
        tempProjectName = System.Text.RegularExpressions.Regex.Replace(tempProjectName, @"[^a-zA-Z0-9_]", "");

        // 开头不能是数字，如果开头是数字，自动在前面加下划线
        if (!string.IsNullOrEmpty(tempProjectName) && char.IsDigit(tempProjectName[0]))
            tempProjectName = "_" + tempProjectName;

        // 显示输入框
        tempProjectName = EditorGUILayout.TextField(tempProjectName);

        GUILayout.Space(10);
        GUILayout.Label("项目类型");

        GUILayout.BeginHorizontal();
        tempIs3D = GUILayout.Toggle(tempIs3D, "3D", "Button", GUILayout.Height(30));
        tempIs3D = !GUILayout.Toggle(!tempIs3D, "2D", "Button", GUILayout.Height(30));
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        GUI.enabled = !string.IsNullOrEmpty(tempProjectName);

        if (GUILayout.Button("下一步", GUILayout.Height(40)))
        {
            UserResource.Instance.ProjectName = tempProjectName;
            UserResource.Instance.is3D = tempIs3D;

            Debug.Log(
                $"[Init] ProjectName={tempProjectName}, Mode={(tempIs3D ? "3D" : "2D")}"
            );
        }

        GUI.enabled = true;
    }



    // =========================
    // Step 1：玩法设计
    // =========================
    private static void OpenGameDesignChat()
    {
        var chat = AIChatWindow.Open("输入玩法设计");

        chat.SetState(
            aiResponseContent: "",
            inputLabel: "请输入玩法设计",
            sendBtnVisible: false,
            confirmBtnVisible: true
        );

        chat.OnConfirm += gameDesign =>
        {
            UserResource.Instance.GameDesign = gameDesign;

            Rect chatRect = chat.position;
            chat.Close();

            OpenSystemDesignRequest(chatRect);
        };
    }

    // =========================
    // Step 2：系统设计
    // =========================
    private static void OpenSystemDesignRequest(Rect chatRect)
    {
        var waitingChat = AIChatWindow.Open("系统设计生成中");
        waitingChat.position = chatRect;

        waitingChat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: false
        );

        string prompt =
            ResourceData.MakeSystemDescriptionPrompt(
                UserResource.Instance.GameDesign
            );

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += systemDesign =>
        {
            UserResource.Instance.SystemDesign = systemDesign;

            waitingChat.SetState(
                aiResponseContent: systemDesign,
                inputLabel: "有什么修改的吗",
                sendBtnVisible: true,
                confirmBtnVisible: true
            );

            waitingChat.OnConfirm += _ =>
            {
                Rect rect = waitingChat.position;
                waitingChat.Close();

                OpenDevelopmentPlanDescriptionRequest(rect);
            };
        };
    }

    // =========================
    // Step 3：开发阶段描述
    // =========================
    private static void OpenDevelopmentPlanDescriptionRequest(Rect chatRect)
    {
        var waitingChat = AIChatWindow.Open("开发阶段生成中");
        waitingChat.position = chatRect;

        waitingChat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: false
        );

        string prompt =
            ResourceData.MakedevelopmentPlanDescription(
                UserResource.Instance.SystemDesign,
                UserResource.Instance.GameDesign
            );

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += description =>
        {
            UserResource.Instance.GameStagesDescription = description;

            waitingChat.SetState(
                aiResponseContent: description,
                inputLabel: "有什么修改的吗",
                sendBtnVisible: true,
                confirmBtnVisible: true
            );

            waitingChat.OnConfirm += _ =>
            {
                Rect rect = waitingChat.position;
                waitingChat.Close();

                OpenDevelopmentPlanJsonRequest(rect);
            };
        };
    }

    // =========================
    // Step 4：最终 JSON
    // =========================
    private static void OpenDevelopmentPlanJsonRequest(Rect chatRect)
    {
        var waitingChat = AIChatWindow.Open("生成最终阶段中");
        waitingChat.position = chatRect;

        waitingChat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: false
        );

        string prompt =
            ResourceData.DevelopePlanMakeJson(
                UserResource.Instance.GameStagesDescription
            );

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += json =>
        {
            UserResource.Instance.makeGameDevelopmentStageByJson(json);

            waitingChat.Close();
            request.Close();

            Debug.Log("[DevelopmentPlan] 完成开发计划生成");
        };
    }
}

[SerializeField]
public class CodeGenerationView
{
    private static Vector2 codeScrollPosition = Vector2.zero;

    // =========================
    // 累加所有 Batch 的生成结果
    // =========================
    private static StringBuilder allCodeResult = new StringBuilder();

    public static void Draw(MainWin mainWin)
    {
        GUILayout.Label("代码生成", EditorStyles.boldLabel);
        GUILayout.Space(10);

        var batches = UserResource.Instance.CurrentStageInfo.CodeBatchs;
        int currentBatch = UserResource.Instance.CurrentStageInfo.currentCodeBatchIndex;

        // =========================
        // 还没有 Code Plan
        // =========================
        if (batches == null || batches.Count == 0)
        {
            GUILayout.Label("还没有生成代码计划");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("进入对话窗口"))
            {
                OpenCodeDesignChat();
            }

            return;
        }

        // =========================
        // Code Batch 列表
        // =========================
        GUILayout.BeginVertical(EditorStyles.helpBox);
        codeScrollPosition = GUILayout.BeginScrollView(
            codeScrollPosition,
            GUILayout.Height(mainWin.position.height - 160)
        );

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

        // =========================
        // 底部按钮
        // =========================
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        bool hasMoreBatch = currentBatch < batches.Count;

        if (hasMoreBatch)
        {
            if (GUILayout.Button("继续生成", GUILayout.Width(140), GUILayout.Height(40)))
            {
                GenerateNextCodeBatch();
            }
        }
        else
        {
            if (GUILayout.Button("下一步", GUILayout.Width(140), GUILayout.Height(40)))
            {
                mainWin.GoToPage(MainPage.PrefabSceneSingleton);
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    // =====================================================
    // Step 1：第一次生成 CodeDesign + CodeBatch 计划
    // =====================================================
    private static void OpenCodeDesignChat()
    {
        var chat = AIChatWindow.Open("代码设计生成中");
        chat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: true
        );

        string prompt = ResourceData.MakeCodeDesignPrompt(
            UserResource.Instance.is3D,
            UserResource.Instance.GameDesign,
            UserResource.Instance.SystemDesign,
            UserResource.Instance.GameStagesDescription,
            UserResource.Instance.gameDevelopementStages[
                UserResource.Instance.currentStageIndex
            ].description
        );

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += content =>
        {
            UserResource.Instance.CurrentStageInfo.CodeDesign = content;

            chat.SetState(
                aiResponseContent: content,
                inputLabel: "有什么修改的吗",
                sendBtnVisible: true,
                confirmBtnVisible: true
            );

            chat.OnConfirm += finalContent =>
            {
                string jsonPrompt =
                    ResourceData.CodeBatchMakeJson(
                        UserResource.Instance.CurrentStageInfo.CodeDesign
                    );

                var batchRequest = requestAiResponse.Open(jsonPrompt);

                batchRequest.OnConfirmed += result =>
                {
                    UserResource.Instance.CurrentStageInfo
                        .makeCodeGenerationBatchByJson(result);

                    UserResource.Instance.CurrentStageInfo.currentCodeBatchIndex = 0;

                    Debug.Log("[CodeGeneration] CodeBatch 计划生成完成");

                    batchRequest.Close();
                    chat.Close();
                    request.Close();
                };
            };
        };
    }

    // =====================================================
    // Step 2：生成下一批代码（累计，最后一次性执行）
    // =====================================================
    private static void GenerateNextCodeBatch()
    {
        var batches = UserResource.Instance.CurrentStageInfo.CodeBatchs;
        int index = UserResource.Instance.CurrentStageInfo.currentCodeBatchIndex;

        if (batches == null || index >= batches.Count)
        {
            Debug.LogWarning("没有更多 Code Batch 可以生成");
            return;
        }

        // 第一个 Batch 时清空累计结果
        if (index == 0)
        {
            allCodeResult.Clear();
        }

        var scripts = batches[index].scripts;

        string prompt = ResourceData.WriteCodeCommandFromScriptName(
            UserResource.Instance.is3D,
            UserResource.Instance.CurrentStageInfo.CodeDesign,
            UserResource.Instance.SystemDesign,
            scripts,
            UserResource.Instance.ProjectName
        );

        Debug.Log($"[CodeGeneration] 执行 Batch {index + 1}");

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += result =>
        {
            bool isLastBatch = index == batches.Count - 1;

            // ===== 累加结果 =====
            allCodeResult.AppendLine($"// ===== Batch {index + 1} =====");
            allCodeResult.AppendLine(result);
            allCodeResult.AppendLine();

            UserResource.Instance.CurrentStageInfo.currentCodeBatchIndex++;
            Debug.Log($"[CodeGeneration] Batch {index + 1} 完成");

            // ===== 最后一个 Batch：一次性执行 =====
            if (isLastBatch)
            {
                Debug.Log("[CodeGeneration] 最后一个 Batch，开始执行全部代码");

                TextCommandExecutor.ExecuteWriteCodeCommands(
                    allCodeResult.ToString()
                );
            }

            request.Close();
        };
    }
}



internal class ScenePrefabSingleton
{
    private static Vector2 scrollPos;

    internal static void Draw(MainWin win)
    {
        GUILayout.Label("Scene Prefab Singleton 设计", EditorStyles.boldLabel);
        GUILayout.Space(10);

        var list = UserResource.Instance.CurrentStageInfo.ScenePrefabSingletonStages;

        // =========================
        // 没有内容：启动 AI 流程
        // =========================
        if (list == null || list.Count == 0)
        {
            GUILayout.Label("尚未生成 Scene Prefab Singleton 设计。");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("生成 Scene Singleton", GUILayout.Height(40)))
            {
                OpenScenePrefabSingletonChat();
            }

            return;
        }

        // =========================
        // 有内容：展示结果
        // =========================
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        foreach (var item in list)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(item.title, EditorStyles.boldLabel);
            GUILayout.Label(item.description, EditorStyles.wordWrappedLabel);
            GUILayout.EndVertical();

            GUILayout.Space(6);
        }

        GUILayout.EndScrollView();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("下一步", GUILayout.Width(160), GUILayout.Height(40)))
        {
            win.GoToPage(MainPage.Prefab);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    // =========================
    // Step 1：描述生成
    // =========================
    private static void OpenScenePrefabSingletonChat()
    {
        var chat = AIChatWindow.Open("Scene Singleton 设计中");
        chat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: false
        );

        string prompt = ResourceData.ScenePrefabSingletonDescription(
            UserResource.Instance.GameDesign,
            UserResource.Instance.SystemDesign,
            UserResource.Instance.currentCodeInfo()
        );

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += content =>
        {
            chat.SetState(
                aiResponseContent: content,
                inputLabel: "需要修改吗？",
                sendBtnVisible: true,
                confirmBtnVisible: true
            );
            UserResource.Instance.CurrentStageInfo.ScenePrefabSingletonDescription = content;
            chat.OnConfirm += finalContent =>
            {
                Rect rect = chat.position;
                chat.Close();
                request.Close();

                OpenScenePrefabSingletonJsonRequest(rect, finalContent);
            };
        };
    }

    // =========================
    // Step 2：JSON 生成
    // =========================
    private static void OpenScenePrefabSingletonJsonRequest(Rect chatRect, string description)
    {
        var waitingChat = AIChatWindow.Open("生成 Scene Singleton JSON");
        waitingChat.position = chatRect;

        waitingChat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: false
        );

        string prompt = ResourceData.MakeScenePrefabSingletonJsonPrompt(UserResource.Instance.CurrentStageInfo.ScenePrefabSingletonDescription);

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += json =>
        {
            UserResource.Instance.CurrentStageInfo
                .makeScenePrefabSingletonByJson(json);

            waitingChat.Close();
            request.Close();

            Debug.Log("[ScenePrefabSingleton] JSON 生成完成");
        };
    }
}

public class PrefabGenerationView
{
    private static Vector2 prefabScrollPosition = Vector2.zero;

    public static void Draw(MainWin win)
    {
        GUILayout.Label("Prefab 设计", EditorStyles.boldLabel);
        GUILayout.Space(10);

        var prefabDesigns = UserResource.Instance.CurrentStageInfo.PrefabDesign;

        if (prefabDesigns == null || prefabDesigns.Count == 0)
        {
            GUILayout.Label("尚未生成 Prefab 设计。");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("进入 Prefab 设计", GUILayout.Height(40)))
            {
                OpenPrefabDesignChat();
            }

            return;
        }

        prefabScrollPosition = GUILayout.BeginScrollView(prefabScrollPosition, GUILayout.ExpandHeight(true));

        for (int i = 0; i < prefabDesigns.Count; i++)
        {
            var prefab = prefabDesigns[i];

            GUI.color = i < UserResource.Instance.CurrentStageInfo.currentPrefabIndex ? Color.gray : Color.white;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(prefab.title, EditorStyles.boldLabel);
            GUILayout.Label(prefab.description, EditorStyles.wordWrappedLabel);
            GUILayout.EndVertical();

            GUI.color = Color.white;
            GUILayout.Space(6);
        }

        GUILayout.EndScrollView();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (UserResource.Instance.CurrentStageInfo.currentPrefabIndex < prefabDesigns.Count)
        {
            if (GUILayout.Button("生成下一个 Prefab", GUILayout.Width(200), GUILayout.Height(40)))
            {
                GenerateNextPrefab();
            }
        }
        else
        {
            if (GUILayout.Button("下一步", GUILayout.Width(140), GUILayout.Height(40)))
            {
                win.GoToPage(MainPage.SceneBuilding);
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    // =========================
    // 打开 Prefab Chat（初始流程）
    // =========================
    private static void OpenPrefabDesignChat()
    {
        var chat = AIChatWindow.Open("Prefab 设计中");
        chat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: false
        );

        string prompt = ResourceData.MakePrefabDesign(
            UserResource.Instance.GameDesign,
            UserResource.Instance.SystemDesign,
            UserResource.Instance.currentCodeInfo(),
            UserResource.Instance.CurrentStageInfo.ScenePrefabSingletonDescription
        );

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += content =>
        {
            chat.SetState(
                aiResponseContent: content,
                inputLabel: "有什么修改的吗",
                sendBtnVisible: true,
                confirmBtnVisible: true
            );
            UserResource.Instance.CurrentStageInfo.CurrentStageAllPrefabDescription = content;
            chat.OnConfirm += finalContent =>
            {
                

                Rect chatRect = chat.position;
                chat.Close();
                request.Close();

                OpenPrefabJsonRequest(chatRect);
            };
        };
    }

    // =========================
    // 生成单个 Prefab
    // =========================
    private static void GenerateNextPrefab()
    {
        var prefabDesigns = UserResource.Instance.CurrentStageInfo.PrefabDesign;
        if (UserResource.Instance.CurrentStageInfo.currentPrefabIndex >= prefabDesigns.Count) return;

        var prefab = prefabDesigns[UserResource.Instance.CurrentStageInfo.currentPrefabIndex];
        string prompt = ResourceData.MakeSinglePrefabJson(
            prefab.title,
            prefab.description,
            UserResource.Instance.CurrentStageInfo.allPrefabCommand,
            UserResource.Instance.currentCodeInfo(),
            ResourceData.ArtAllItemsName()
        );

        // 弹出 request window
        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += content =>
        {
            var createdObject = TextCommandExecutor.ExecuteGameobjectCommand(content);
            UserResource.Instance.CurrentStageInfo.allPrefabCommand += content+"\n\n";
            Debug.Log($"[PrefabGeneration] 生成 Prefab：{prefab.title}");

#if UNITY_EDITOR
            // 在 Scene 里选中对象
            if (createdObject != null)
                Selection.activeGameObject = createdObject;
            SceneView.lastActiveSceneView.FrameSelected();
#endif


            // 更新索引
            UserResource.Instance.CurrentStageInfo.currentPrefabIndex++;

            // 如果最后一个 prefab，关闭 request window
            if (UserResource.Instance.CurrentStageInfo.currentPrefabIndex >= prefabDesigns.Count)
            {
                request.Close();
            }
            else
            {
                // 否则可以保持 request window打开或等待下一次点击
                request.Close();
            }
        };
    }

    // =========================
    // Step 2：生成 Prefab JSON
    // =========================
    private static void OpenPrefabJsonRequest(Rect chatRect)
    {
        var waitingChat = AIChatWindow.Open("生成 Prefab JSON");
        waitingChat.position = chatRect;

        waitingChat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: false
        );

        string prompt = ResourceData.MakePrefabDesciptionJson(UserResource.Instance.CurrentStageInfo.CurrentStageAllPrefabDescription);

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += json =>
        {
            var list = UserResource.Instance.CurrentStageInfo.makePrefabDesignByJson(json);
            Debug.Log($"[PrefabGeneration] Prefab JSON 生成完成，总数：{list?.Count ?? 0}");

            waitingChat.Close();
            request.Close();
        };
    }
}




public class SceneBuildingView
{
    private static Vector2 sceneScrollPosition = Vector2.zero;

    public static void Draw(MainWin win)
    {
        GUILayout.Label("场景构建", EditorStyles.boldLabel);
        GUILayout.Space(10);

        var sceneSteps = UserResource.Instance.CurrentStageInfo.MakeSceneStages;

        // =========================
        // 尚未生成场景设计
        // =========================
        if (sceneSteps == null || sceneSteps.Count == 0)
        {
            GUILayout.Label("尚未生成场景设计方案。");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("进入场景设计", GUILayout.Height(40)))
            {
                OpenSceneDesignChat();
            }

            return;
        }

        // =========================
        // 展示 Step Summary（title + description）
        // =========================
        sceneScrollPosition = GUILayout.BeginScrollView(sceneScrollPosition, GUILayout.ExpandHeight(true));

        for (int i = 0; i < sceneSteps.Count; i++)
        {
            var step = sceneSteps[i];

            GUI.color = i < UserResource.Instance.CurrentStageInfo.currentSceneIndex ? Color.gray : Color.white;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(step.title, EditorStyles.boldLabel);
            GUILayout.Label(step.description, EditorStyles.wordWrappedLabel);
            GUILayout.EndVertical();

            GUI.color = Color.white;
            GUILayout.Space(6);
        }

        GUILayout.EndScrollView();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // =========================
        // 逐步生成 Scene JSON
        // =========================
        if (UserResource.Instance.CurrentStageInfo.currentSceneIndex < sceneSteps.Count)
        {
            if (GUILayout.Button("生成下一个步骤", GUILayout.Width(200), GUILayout.Height(40)))
            {
                GenerateNextSceneStep();
            }
        }
        else
        {
            if (GUILayout.Button("下一步", GUILayout.Width(140), GUILayout.Height(40)))
            {
                win.GoToPage(MainPage.DebugAcceptance);
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    // ============================================================
    // Step 1：自然语言 · 场景设计确认
    // ============================================================
    private static void OpenSceneDesignChat()
    {
        var chat = AIChatWindow.Open("场景设计中");
        chat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: false
        );
        string prompt = ResourceData.MakeGenerateScenePlanDescription(
            UserResource.Instance.GameDesign,
            UserResource.Instance.GameStagesDescription,
             UserResource.Instance.CurrentStageInfo.ScenePrefabSingletonDescription,
            UserResource.Instance.gameDevelopementStages[UserResource.Instance.currentStageIndex].description,
            UserResource.Instance.CurrentStageInfo.allPrefabCommand,
            ResourceData.ArtAllItemsName()
            );
        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += content =>
        {
            chat.SetState(
                aiResponseContent: content,
                inputLabel: "需要调整哪些地方？",
                sendBtnVisible: true,
                confirmBtnVisible: true
            );
            UserResource.Instance.sceneDesignDescription = content;
            chat.OnConfirm += finalContent =>
            {
               

                Rect chatRect = chat.position;
                chat.Close();
                request.Close();

                OpenSceneStepSummaryRequest(chatRect);
            };
        };
    }

    // ============================================================
    // Step 2：生成 Step Summary（title + description）
    // ============================================================
    private static void OpenSceneStepSummaryRequest(Rect chatRect)
    {
        var waitingChat = AIChatWindow.Open("生成场景步骤摘要");
        waitingChat.position = chatRect;

        waitingChat.SetState(
            aiResponseContent: "输入中...",
            inputLabel: "",
            sendBtnVisible: false,
            confirmBtnVisible: false
        );

        string prompt = ResourceData.sceneDesignMakeJsonPrompt(
            UserResource.Instance.sceneDesignDescription
        );

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += json =>
        {
            UserResource.Instance.CurrentStageInfo.MakeSceneDescription = json;
            var list = UserResource.Instance.CurrentStageInfo.makeSceneDesignByJson(json);
            Debug.Log($"[SceneBuilding] 场景步骤生成完成，总数：{list?.Count ?? 0}");

            waitingChat.Close();
            request.Close();
        };
    }

    // ============================================================
    // Step 3：生成单个 Scene / Prefab Build JSON
    // ============================================================
    private static void GenerateNextSceneStep()
    {
        var steps = UserResource.Instance.CurrentStageInfo.MakeSceneStages;
        if (UserResource.Instance.CurrentStageInfo.currentSceneIndex >= steps.Count) return;

        var step = steps[UserResource.Instance.CurrentStageInfo.currentSceneIndex];
        string prompt = ResourceData.SceneStepToJson(
            UserResource.Instance.sceneDesignDescription,
            step.description,
            ResourceData.ArtAllItemsName(),
            UserResource.Instance.CurrentStageInfo.allPrefabCommand
        );

        var request = requestAiResponse.Open(prompt);

        request.OnConfirmed += content =>
        {
            // 执行 Prefab / Scene 构建命令
            TextCommandExecutor.ExecuteGameobjectCommand(content);
            Debug.Log($"[SceneBuilding] 已执行步骤：{step.title}");

            UserResource.Instance.CurrentStageInfo.currentSceneIndex++;

            request.Close();
        };
    }
}


public class DebugAcceptanceView
{
    public static void Draw(MainWin win)
    {
        //有一个按钮，生成
    }

}