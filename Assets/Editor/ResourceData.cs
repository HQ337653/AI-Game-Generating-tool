using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
namespace AiHelper
{
    [CreateAssetMenu(fileName = "ResourceData", menuName = "Resources/ResourceData", order = 1)]
    public class ResourceData : ScriptableObject
    {
        
        public static string CodeDirectory = "Assets/Scripts";

        public static string ArtDirectory = "???";
        public static string ProjectScriptLocation(string projectName)
        {
            return CodeDirectory+"/" + projectName;
        }
        public static string ArtAllItemsName()
        {
            return PrefabRegistry.Instance.GetAllArtPrefabNames();
        }


        public static string MakeSystemDescriptionPrompt(string gameplayDescription)
        {
            return gameSysetmPrompt + "\n[玩法介绍]\n" + gameplayDescription;
        }

        public static string MakedevelopmentPlanDescription(string system,string gamedesign)
        {
            return developmentPlanDescriptionPrompt + "\n[游戏系统]\n" + system + "\n[游戏设计]\n" + gamedesign;
        }

        public static string DevelopePlanLessStage(string CurrentStageDesign, string systemDesign)
        {
            return "我认为现在这个开发计划步骤太多了，帮我减少\n### 强制规则（非常重要）：\r\n1. **从具体到抽象**  \r\n   - 先实现最低抽象程度的系统  \r\n   - 后面的阶段只能建立在前一阶段已经“能跑”的基础上\r\n\r\n2. **每一个阶段都必须是一个“可运行版本”**\r\n   - 明确说明：这个阶段的版本玩家能做什么\r\n   - 明确说明：这个阶段主要是为了验证哪些系统是否工作正常\r\n\r\n3. **禁止跨抽象程度**\r\n   - 一个阶段只能使用【同一抽象程度】或【已完成的更低抽象程度】的系统\r\n   - 不能提前引入更高层的流程控制、UI 或全局管理\r\n\r\n4. **阶段数量适中**\r\n   - 不追求一步一个系统\r\n   - 如果同一抽象层的系统强相关，可以自然地合并在一个阶段中\r\n\r\n5. **只用自然语言**\r\n   - 不要输出 JSON\r\n   - 不要使用编号以外的结构化格式\r\n   - 不要写代码\r\n   - 不要讨论美术或实现细节\r\n\r\n---\r\n\r\n### 输出格式要求：\r\n\r\n请按以下结构输出：\r\n\r\n开发阶段 1：\r\n- 阶段目标（用一句话概括这个版本“存在的意义”）\r\n- 这个版本玩家可以做什么\r\n- 本阶段重点验证的系统（用自然语言描述，不要简单列系统名）\r\n- 为什么这些系统必须在这个阶段一起完成\r\n\r\n开发阶段 2：\r\n（同上）\r\n\r\n开发阶段 3：\r\n（同上）" + "\n[现在开发步骤]\n" + CurrentStageDesign + "\n[现在游戏系统]\n" + systemDesign;
        }

        public static string DevelopePlanMoreStage(string CurrentStageDesign, string systemDesign)
        {
            return "我认为现在这个开发计划步骤太少了，帮我增加\n### 强制规则（非常重要）：\r\n1. **从具体到抽象**  \r\n   - 先实现最低抽象程度的系统  \r\n   - 后面的阶段只能建立在前一阶段已经“能跑”的基础上\r\n\r\n2. **每一个阶段都必须是一个“可运行版本”**\r\n   - 明确说明：这个阶段的版本玩家能做什么\r\n   - 明确说明：这个阶段主要是为了验证哪些系统是否工作正常\r\n\r\n3. **禁止跨抽象程度**\r\n   - 一个阶段只能使用【同一抽象程度】或【已完成的更低抽象程度】的系统\r\n   - 不能提前引入更高层的流程控制、UI 或全局管理\r\n\r\n4. **阶段数量适中**\r\n   - 不追求一步一个系统\r\n   - 如果同一抽象层的系统强相关，可以自然地合并在一个阶段中\r\n\r\n5. **只用自然语言**\r\n   - 不要输出 JSON\r\n   - 不要使用编号以外的结构化格式\r\n   - 不要写代码\r\n   - 不要讨论美术或实现细节\r\n\r\n---\r\n\r\n### 输出格式要求：\r\n\r\n请按以下结构输出：\r\n\r\n开发阶段 1：\r\n- 阶段目标（用一句话概括这个版本“存在的意义”）\r\n- 这个版本玩家可以做什么\r\n- 本阶段重点验证的系统（用自然语言描述，不要简单列系统名）\r\n- 为什么这些系统必须在这个阶段一起完成\r\n\r\n开发阶段 2：\r\n（同上）\r\n\r\n开发阶段 3：\r\n（同上）" + "\n[现在开发步骤]\n" + CurrentStageDesign + "\n[现在游戏系统]\n" + systemDesign;
        }
        public static string DevelopePlanMakeJson(string developementPlan)
        {
            return MakeDevelopmentPlanJson + developementPlan;
        }

        public static string CodeBatchMakeJson(string codeDesign)
        {
            return MakeCodeGenerationBatchJson + "\n\n[代码设计与依赖关系]\n" + codeDesign;
        }
        public static string MakePrefabDesign(string GameplayDesign,string SystemAndItem,string ScriptDesign)
        {
            var s = MakePrefabDesignPrompt + "\n\n【Gameplay Description】\n\n" + GameplayDesign + "\n开发计划\n" + UserResource.Instance.GameStagesDescription + "\n\n现在环节\n\n：" + UserResource.Instance.gameDevelopementStages[UserResource.Instance.currentStageIndex].description +"\n\n【System & Item List】\n\n" + SystemAndItem + "\n\n【Script Responsibilities (Reference Only)】\n\n"+ ScriptDesign;
            return s;
        }

        public static string ModificationPrompt(string usercontent)
        {
            var s = "对于这个prefab设计，我有一些意见： " + usercontent;
                return s;
        }
        public static string WriteCodeCommandFromScriptName(bool is3D,string codeDesign, string systemDesign,List<string> codeNames,string projectName)
        {
            string scriptList = "";
            foreach (var scriptName in codeNames)
            {
                scriptList += scriptName + "\n";
            }

            string scriptPath = $"{CodeDirectory}/{projectName}";
            string EnviromentInfo = "\n[Unity环境内容]\n物理系统：" + UnityPackages;
            return GenerateWriteCodeCommand.Replace("{SCRIPT_PATH}", scriptPath)
     + (is3D ? "【游戏类型：3D 游戏】" : "【游戏类型：2D 游戏】")
     + "\n[环境信息]\n"
     + EnviromentInfo
     + "\n[代码设计]\n"
     + codeDesign
     + "\n\n[系统描述]\n"
     + systemDesign
     + "\n\n需要生成的脚本为：\n"
     + scriptList;
        }

        public static string MakeCodeDesignPrompt(
        bool is3D,
        string gameplayDesign,
         string systemDesign,
        string gameStageDesign,
         string currentStateDescription)
        {
            // 假设 UnityPackages 是一个全局或类内的字符串变量，记录了 Unity 环境信息
            string EnviromentInfo = "\n[Unity环境内容]\n物理系统：" + UnityPackages;

            // 根据是否3D生成不同的提示信息
            string dimensionInfo = is3D ? "这是一个3D游戏。" : "这是一个2D游戏。";

            // 拼接最终设计提示
            string prompt = MakeCodeDesign +
                            $"\n\n{dimensionInfo}\n" +
                            $"玩法设计：{gameplayDesign}\n" +
                            $"系统设计：{systemDesign}\n" +
                            $"游戏阶段设计：{gameStageDesign}\n" +
                            $"当前状态描述：{currentStateDescription}\n" +
                            $"{EnviromentInfo}";

            return prompt;
        }
        public static string MakeGenerateScenePlanDescription(string prefabinfo, string gamedesign, string developmentplan, string developmentStage, string artList)
        {
            return GenerateSceneMakingDescription +
                   "【prefab信息】\r\n" + prefabinfo + "\r\n" +
                   "【玩法设计】\r\n" + gamedesign + "\r\n" +
                   "【开发计划】\r\n" + developmentplan + "\r\n" +
                   "【开发阶段】\r\n" + developmentStage + "\r\n" +
                   "【美术列表】\r\n" + artList;
        }
        internal static string CodeDesignModificationPrompt(string codeUserInput,string aiResponse, string gameplayDesign, string systemDesign, string gameStageDesign, string currentStateDiscription)
        {
            var EnviromentInfo = "\n[unity环境内容]" + UnityPackages;
            return "我对现在的脚本设计有些意见\n"+ codeUserInput + "\n【现在的设计】\n" + aiResponse+"\n【玩法描述】\n" + gameplayDesign + "\n【系统清单】\n" + systemDesign + "\n【开发计划描述】\n" + gameStageDesign + "【当前阶段】：" + currentStateDiscription +EnviromentInfo;
        }
        internal static string MakePrefabDesciptionJson(string content)
        {
            string prompt =
                "请根据以下 Prefab 设计内容生成 JSON 列表，每个元素代表一个 Prefab，" +
                "每个 Prefab 至少包含字段：\n" +
                "  - title: Prefab 名称\n" +
                "  - description: Prefab 描述\n\n" +
                "特别要求：\n" +
                "1. 如果某个 Prefab 依赖其他 Prefab（例如引用了其他 GameObject 或组件），请确保被依赖的 Prefab 出现在依赖它的 Prefab 之前。\n" +
                "2. JSON 顶层必须是一个对象，包含字段 \"stages\"，它是一个数组，数组元素就是所有 Prefab。\n" +
                "3. 输出的 JSON 必须严格符合 Unity JsonUtility 可解析的格式，不要包含任何额外说明、注释或文本。\n\n" +
                "示例输出格式如下：\n" +
                "{\n" +
                "  \"stages\": [\n" +
                "    {\n" +
                "      \"title\": \"PrefabA\",\n" +
                "      \"description\": \"这是第一个 Prefab，其他 Prefab 可以依赖它。\"\n" +
                "    },\n" +
                "    {\n" +
                "      \"title\": \"PrefabB\",\n" +
                "      \"description\": \"这个 Prefab 依赖 PrefabA。\"\n" +
                "    }\n" +
                "  ]\n" +
                "}\n\n" +
                "请根据以下 Prefab 设计内容生成 JSON：\n" +
                content;

            return prompt;
        }

        internal static string MakeSinglePrefabJson(
            string targetPrefab,
            string targetPrefabDecription,
            string allPrefabs,
            string scriptDescription,
            string artAssets)
        {
            return singlePrefabJsonPrompt +
                   $"\n\n【目标prefab】\n{targetPrefab},{targetPrefabDecription}\r\n" +
                   $"【所有prefab id】{allPrefabs}\r\n" +
                   $"【脚本简介】{scriptDescription}\r\n" +
                   $"【美术素材】{artAssets}";
        }
        public static string SceneStepToJson(
    string sceneStepDesign,
    string targetStep,
    string artResources,string allPrefab)
        {
           return SceneStepToJsonPrompt+ 
        "\r\n【场景步骤设计】\r\n" +
        sceneStepDesign +
        "\r\n\r\n【目标步骤】\r\n" +
        targetStep +
        "\r\n\r\n【美术资源】\r\n" +
        artResources+
         "\r\n\r\n【已经生成的prefab】\r\n" +
         allPrefab
         ;

        }
        public static string SceneStepToJsonPrompt = "你是 Unity 的地图 设计专家。\r\n\r\n我会提供以下信息：\r\n美术资源、已经生成的 prefab 清单\r\n\r\n\r\n你的任务：\r\n1. 为地图生成的指定步骤生成结构方案\r\n2. 输出 Prefab Build JSON，严格遵守以下规则：\r\n   • 步骤类型仅允许：create_object, add_child, add_component, set_property\r\n\r\n   • 每个 GameObject 必需 Unity 内置组件必须显式列出，例如：Transform, Rigidbody, BoxCollider, MeshRenderer 等\r\n\r\n   • 自定义脚本组件也必须显式列出\r\n\r\n   • 指定属性字段必须用 set_property 设置\r\n\r\n   • 引用字段必须使用 \"object:ID\" 或 \"component:ID:ComponentName\" 格式\r\n\r\n   • 对于美术资源：\r\n\r\n     ◦ 如果资源类型是 prefab，生成步骤时用 add_child，并在 add_component 或 set_property 中标记为引用 art prefab（请注意，一个prefab所有内容都必须在一个root里面，所以只能在美术作为root object的时候使用create_object）\r\n\r\n     ◦ 如果资源类型是 sprite 或其他组件化资源，将其挂载在对应的组件（例如 SpriteRenderer 的 sprite 字段）\r\n\r\n     ◦ 所有美术资源必须通过实例化（Instantiate）生成，不直接移动原型\r\n\r\n     ◦ 每个新建的gameobject，不管是child还是root，都必须有一个全局唯一id，可被同一 prefab 内其他字段引用，也可以被不同 prefab 内其他字段引用。因此得保证所有prefab每一个新建的gameobjet都有不一样的id，避免比如 visual，hitpoint，这种会重复的，而是做成playervisual，enemyhitpoint这样具体的id\r\n\r\n你可以选择新建物品（比如制作地形）或者移动物品（比如把玩家移动到DefaultSceneDynamicItemRoot下，然后设置transform让玩家站在出生点），你也可以同时做这两件事\r\n\r\n你会拿到一份生成完的prefab，你不能instantiate这些prefab，你只能移动它们\r\n3. 不生成任何 C# 代码\r\n4. 不执行 Instantiate 或运行时逻辑\r\n5. 输出 JSON 必须可以顺序执行构建 prefab\r\n6. 输出仅生成指定的单个 prefab 描述\r\n7. 使用已有 prefab / 游戏实体清单以及提供的美术资源清单作为参考\r\n8. 不生成新的 prefab 名称或新实体\r\n9. 输出 JSON 中的步骤顺序必须遵循：create_object → add_child → add_component → set_property\r\n10. 每个步骤都必须包含唯一 id，用于引用同一 prefab 内对象或组件\r\n11. 输出 JSON 必须保持可解析结构，适合新版 PrefabJsonExecutor 执行\r\n12. Transform 或其他 Unity 内置组件必须显式列出\r\n13. 所有组件字段必须可直接设置\r\n14. 不允许生成新的类或 prefab\r\n15. 不生成任何运行时代码\r\n16. 输出严格 JSON，不允许多余文本\r\n17. 美术资源必须明确标注类型，并根据类型生成对应步骤和引用\r\n18. 对 sprite 或其他资源，字段引用必须用 set_property 指定组件属性，例如 SpriteRenderer.sprite = \"object:ID\"\r\n19. 如果需要用到world Space  canvas，在这个步骤生成完整内容也处理好所有引用\r\n20. 当需要引用Screen space canvas或者需要在它下面新建任何object，不用生成这种物品，也不需要对涉及这个物品的字段写set property\r\n21. json中Unity 数据类型统一转成字符串\r\n• Vector3 → \"x,y,z\"\r\n• Quaternion → \"x,y,z,w\"\r\n• Vector2 → \"x,y\"\r\n• Color / Color32 → \"r,g,b,a\"\r\n22. 特殊物品id：DefaultSceneStaticItemRoot，DefaultSceneDynamicItemRoot，DefalutCanvas，DefalutVirtualCamera（对应gameobject默认已经存在于场景中（0，0，0）的位置）\r\n23. 保证所有步骤结束后，所有单例物品都是DefaultSceneStaticItemRoot和DefaultSceneDynamicItemRoot的childobject\r\n示例输出格式（严格 JSON）：\r\n\r\n{\r\n  \"prefabName\": \"Item_HealthPotion\",\r\n  \"root\": {\r\n    \"id\": \"rootHealthPotion\",\r\n    \"name\": \"HealthPotion\"\r\n  },\r\n  \"steps\": [\r\n    { \"type\": \"create_object\", \"id\": \"rootHealthPotion\", \"name\": \"HealthPotion\" },\r\n    { \"type\": \"add_component\", \"target\": \"root\", \"component\": \"Transform\" },\r\n    { \"type\": \"add_component\", \"target\": \"root\", \"component\": \"Item\" },\r\n    { \"type\": \"add_child\", \"id\": \"visual\", \"parent\": \"root\", \"name\": \"Visual\" },\r\n    { \"type\": \"add_component\", \"target\": \"visual\", \"component\": \"MeshRenderer\" },\r\n    { \"type\": \"add_child\", \"id\": \"collider\", \"parent\": \"root\", \"name\": \"HitBox\" },\r\n    { \"type\": \"add_component\", \"target\": \"collider\", \"component\": \"BoxCollider\" },\r\n    { \"type\": \"set_property\", \"target\": \"collider\", \"component\": \"BoxCollider\", \"property\": \"size\", \"value\": [1.0, 1.0, 1.0] },\r\n    { \"type\": \"add_child\", \"id\": \"art_branch_01\", \"parent\": \"root\", \"name\": \"Branch_01\" },\r\n    { \"type\": \"add_component\", \"target\": \"art_branch_01\", \"component\": \"Transform\" },\r\n    { \"type\": \"set_property\", \"target\": \"art_branch_01\", \"component\": \"MeshRenderer\", \"property\": \"material\", \"value\": \"object:Branch_01\" }\r\n  ]\r\n}\r\n\r\n如果不是在新建prefab，你可以这样填写参数\"root\": {\r\n    \"id\": \"Adjustment\",\r\n    \"name\": \"Adjustment\"\r\n  },\r\n\r\nPrefabJson 命令格式说明\r\n\r\n1. create_object\r\n   • 用途：在 Prefab 内创建一个新的空 GameObject 或根对象\r\n\r\n   • 必填字段：\r\n\r\n       type: \"create_object\"\r\n       id: 唯一标识符，用于引用该对象\r\n       name: GameObject 名称\r\n   • 示例：\r\n\r\n       { \"type\": \"create_object\", \"id\": \"root\", \"name\": \"HealthPotion\" }\r\n\r\n2. add_child\r\n   • 用途：在父对象下添加一个子 GameObject，可以是空对象或实例化的美术 prefab\r\n\r\n   • 必填字段：\r\n\r\n       type: \"add_child\"\r\n       id: 唯一标识符，用于引用该对象\r\n       parent: 父对象的 id\r\n       name: 子对象名称\r\n   • 备注：\r\n\r\n       ◦ 如果 name 对应的美术资源 prefab，Executor 会实例化它\r\n\r\n       ◦ 空对象时直接创建 GameObject\r\n\r\n   • 示例：\r\n\r\n       { \"type\": \"add_child\", \"id\": \"visual\", \"parent\": \"root\", \"name\": \"Visual\" }\r\n\r\n3. add_component\r\n   • 用途：给指定对象添加组件（Unity 内置组件或自定义脚本）\r\n\r\n   • 必填字段：\r\n\r\n       type: \"add_component\"\r\n       target: 对象 id\r\n       component: 组件名称\r\n   • 备注：\r\n\r\n       ◦ Transform、Rigidbody、Collider 等内置组件必须显式添加\r\n\r\n       ◦ 自定义脚本组件也必须显式添加\r\n\r\n   • 示例：\r\n\r\n       { \"type\": \"add_component\", \"target\": \"visual\", \"component\": \"MeshRenderer\" }\r\n\r\n4. set_property\r\n   • 用途：设置组件字段或属性的值\r\n\r\n   • 必填字段：\r\n\r\n       type: \"set_property\"\r\n       target: 对象 id\r\n       component: 组件名称\r\n       property: 属性或字段名称\r\n       value: 设置值\r\n   • 备注：\r\n\r\n       ◦ 基本类型（int、float、bool、string）可直接写\r\n\r\n       ◦ Vector3 用数组 [x, y, z] 表示\r\n\r\n       ◦ 引用其他对象或组件（可以同一个prefab内部引用，也可以使用objectid引用外部prefab的信息）：\r\n\r\n           ▪ 对象引用: \"object:ID\"\r\n\r\n           ▪ 组件引用: \"component:ID:ComponentName\"\r\n\r\n       ◦ 美术资源 sprite 或材质通过 set_property 指定给对应组件字段\r\n\r\n   • 示例：\r\n\r\n       { \"type\": \"set_property\", \"target\": \"collider\", \"component\": \"BoxCollider\", \"property\": \"size\", \"value\": [1.0, 1.0, 1.0] }\r\n       { \"type\": \"set_property\", \"target\": \"art_branch_01\", \"component\": \"MeshRenderer\", \"property\": \"material\", \"value\": \"object:Branch_01\" }\r\n5. move\r\n\t• 调整已存在 GameObject 的父子关系\r\n\t• 不创建、不销毁、不修改组件或属性\r\n\t• 可以跨prefab移动\r\n \t{\"type\": \"move\",\"target\": \"enemySpawnerid\",\"parent\": \"Managersid\"}\r\n\r\n命令执行顺序：\r\n1. create_object\r\n2. add_child\r\n3. add_component\r\n4. set_property\r\n5. move\r\n注意事项：\r\n• 每个新建的物品必须先新建一个gameobject作为root\r\n\r\n• 每个 GameObject 必须有唯一 id\r\n\r\n• 所有美术资源实例必须通过实例化生成，不直接使用原型\r\n\r\n• 所有引用必须使用 \"object:ID\" 或 \"component:ID:ComponentName\"\r\n\r\n• Transform 或其他 Unity 内置组件必须显式列出\r\n\r\n• JSON 必须严格按顺序执行，保证 命令可以执行成功";
        internal static string sceneDesignMakeJsonPrompt(string sceneDesignDescription)
        {
           return SceneMakeJson + "\n\n[scene制作计划]\n\n"+ sceneDesignDescription;
        }

        public static string SceneMakeJson = "你是一个结构化整理助手。\r\n\r\n我将提供一组【已经确认的游戏开发阶段的自然语言描述】。\r\n这些阶段已经过设计，不需要你重新规划或优化。\r\n\r\n你的任务是：\r\n**仅将这些自然语言阶段，转换为一个 JSON 结构，用于后续自动化处理。**\r\n\r\n⚠️ 严格规则（非常重要）：\r\n1. **不要新增、删除或合并任何内容**\r\n\r\n\r\n---\r\n\r\n### JSON 结构要求（必须严格遵守）：\r\n\r\n请使用以下 JSON 格式（字段名不可更改）：\r\n\r\n{\r\n  \"stages\": [\r\n    {\r\n      \"title\": \"总结\",\r\n      \"description\": \"内容\"\r\n    },\r\n    {\r\n      \"title\": \"总结\",\r\n      \"description\": \"内容\"\r\n    }\r\n  ]\r\n}\r\n\r\n\r\n- `title`：为该阶段提炼一个**简短、概括性的标题**\r\n";

        // 公开属性，获取唯一实例
        public static ResourceData Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 尝试从资源中加载UserResource实例
                    _instance = Resources.Load<ResourceData>("ResourceData");
                    if (_instance == null)
                    {
                        Debug.LogError("UserResource instance not found. Make sure it's placed under a Resources folder.");
                    }
                }
                return _instance;
            }
        }

        private static string MakeScenePlanIntoJsonPrompt =
"你是一个结构化整理助手。\r\n\r\n" +
"我将提供一组【已经确认的游戏场景规划的自然语言描述】。\r\n" +
"这些场景已经过设计，不需要你重新设计、拆分或合并。\r\n\r\n" +
"你的任务是：\r\n" +
"**仅将这些自然语言场景说明，转换为一个 JSON 结构，用于后续自动化处理。**\r\n\r\n" +
"⚠️ 严格规则（非常重要）：\r\n" +
"1. **不要新增、删除或合并场景**\r\n" +
"2. **不要引入新的玩法、系统或设计概念**\r\n" +
"3. **不要跨场景重组内容**\r\n" +
"4. **不要评价或优化场景设计**\r\n" +
"5. 你只能做“整理”和“转写”\r\n\r\n" +
"---\r\n\r\n" +
"### JSON 结构要求（必须严格遵守）：\r\n\r\n" +
"请使用以下 JSON 格式（字段名不可更改）：\r\n\r\n" +
"{\r\n" +
"  \"scenes\": [\r\n" +
"    {\r\n" +
"      \"title\": \"场景的简短标题\",\r\n" +
"      \"description\": \"完整描述该场景在可运行版本中的内容。需要清楚说明：这是一个什么场景，玩家在该场景中能看到什么、能做什么，以及该场景主要承载哪些已经存在的玩法或系统。\"\r\n" +
"    },\r\n" +
"    {\r\n" +
"      \"title\": \"场景的简短标题\",\r\n" +
"      \"description\": \"完整描述该场景在可运行版本中的内容。需要清楚说明：这是一个什么场景，玩家在该场景中能看到什么、能做什么，以及该场景主要承载哪些已经存在的玩法或系统。\"\r\n" +
"    }\r\n" +
"  ]\r\n" +
"}\r\n\r\n" +
"- `title`：为该场景提炼一个**简短、概括性的标题**\r\n" +
"- `description`：\r\n" +
"  - 必须完整覆盖该场景的内容与功能\r\n" +
"  - 必须明确这是一个**可进入、可运行的场景**\r\n" +
"  - 使用一段完整的自然语言描述，不要拆成列表\r\n\r\n" +
"---\r\n\r\n" +
"### 重要内容约束：\r\n" +
"- 每个场景的描述 **只能来自该场景的自然语言说明**\r\n" +
"- 不允许提前引入其他场景中的内容或系统\r\n" +
"- 必须保持原始描述的信息顺序与表达层级，不做抽象或提升\r\n\r\n" +
"---\r\n\r\n" +
"现在，请根据下面提供的【场景规划自然语言描述】，生成最终的 JSON。\r\n\r\n" +
"【场景规划自然语言描述】：";

        private static ResourceData _instance;
        private static string singlePrefabJsonPrompt = "你是 Unity 的prefab 设计专家。\r\n\r\n我会提供以下信息：\r\n美术资源、已经生成的 prefab 清单\r\n\r\n1.思考prefab是否涉及ui\r\n\r\n如果这个prefab中搭载的脚本有字段需要引用UI\r\n\r\n允许直接在 Canvas 下新建 GameObject（禁止新建canvas，禁止修改canvas属性），也可以使用在默认位置新建root，在canvas下面新建UI，然后进行适当的引用，这种情况下允许先在默认地方创建object挂载脚本，再直接对DefaultCanvas进行addchild\r\n\r\n如果是 World Space UI：\r\n\r\n必须完整生成 Canvas、CanvasScaler、GraphicRaycaster，并正确设置 world space 相关属性\r\n\r\n如果是 Screen Space Canvas 或其子物体：\r\n不要新建 Canvas\r\n\r\n不要对 Canvas 本身设置任何属性\r\n\r\n只在其下新建 GameObject，并添加合适组件（RectTransform、Image、Text、Button 等）\r\n\r\nUI 相关对象必须使用 RectTransform\r\n\r\nUI 美术资源（sprite）必须通过 set_property 赋值到对应组件字段（如 Image.sprite）\r\n\r\n\r\n\r\n2. 输出 Prefab Build JSON，严格遵守以下规则：\r\n   • 步骤类型仅允许：create_object, add_child, add_component, set_property\r\n\r\n   • 每个 GameObject 必需 Unity 内置组件必须显式列出，例如：Transform, Rigidbody, BoxCollider, MeshRenderer 等\r\n\r\n   • 自定义脚本组件也必须显式列出\r\n\r\n   • 指定属性字段必须用 set_property 设置\r\n\r\n   • 引用字段必须使用 \"object:ID\" 或 \"component:ID:ComponentName\" 格式\r\n\r\n   • 对于美术资源：\r\n\r\n     ◦ 如果资源类型是 prefab，生成步骤时用 add_child，并在 add_component 或 set_property 中标记为引用 art prefab（请注意，一个prefab所有内容都必须在一个root里面，所以只能在美术作为root object的时候使用create_object）\r\n\r\n     ◦ 如果资源类型是 sprite 或其他组件化资源，将其挂载在对应的组件（例如 SpriteRenderer 的 sprite 字段）\r\n\r\n     ◦ 所有美术资源必须通过实例化（Instantiate）生成，不直接移动原型\r\n\r\n     ◦ 每个新建的gameobject都必须有一 id，可被同一 prefab 内其他字段引用，也可以被不同 prefab 内其他字段引用，每个prefab在此阶段只会实例化一次，因此实例化后id与json内一致\r\n\r\n3注意：这些prefab不会在运行前被复制\r\n\t如果对象为子弹或者其他这类游戏中会被复制的物品，可以直接创建一个\r\n\t如果对象是地图边界空气墙这种在游戏中不会被复制，而是需要在场景设置阶段摆放的物品，就一次生成场景搭建阶段所有需要的。比如如果需要四个墙围住场景，现在就生成四个墙\r\n\r\n特殊id：DefaultCanvas（已存在）\r\n\r\n3. 不生成任何 C# 代码\r\n4. 不执行 Instantiate 或运行时逻辑\r\n5. 输出 JSON 必须可以顺序执行构建 prefab\r\n7. 使用已有 prefab / 游戏实体清单以及提供的美术资源清单作为参考\r\n8. 不生成新的 prefab 名称或新实体\r\n9. 输出 JSON 中的步骤顺序必须遵循：create_object → add_child → add_component → set_property\r\n10. 每个步骤都必须包含唯一 id，用于引用同一 prefab 内对象或组件\r\n11. 输出 JSON 必须保持可解析结构，适合新版 PrefabJsonExecutor 执行\r\n12. Transform 或其他 Unity 内置组件必须显式列出\r\n13. 所有组件字段必须可直接设置\r\n14. 不允许生成新的类或 prefab\r\n15. 不生成任何运行时代码\r\n16. 输出严格 JSON，不允许多余文本\r\n17. 美术资源必须明确标注类型，并根据类型生成对应步骤和引用\r\n18. 对 sprite 或其他资源，字段引用必须用 set_property 指定组件属性，例如 SpriteRenderer.sprite = \"object:ID\"\r\n19. 如果需要用到world Space  canvas，在这个步骤生成完整内容也处理好所有引用\r\n20. 当需要引用Screen space canvas或者需要在它下面新建任何object，不用生成这种物品，也不需要对涉及这个物品的字段写set property\r\n21. json中Unity 数据类型统一转成字符串\r\n• Vector3 → \"x,y,z\"\r\n• Quaternion → \"x,y,z,w\"\r\n• Vector2 → \"x,y\"\r\n• Color / Color32 → \"r,g,b,a\"\r\n22. 范围攻击需要特效的情况可以用美术里的透明unitybox来展示范围\r\n示例输出格式（严格 JSON）：\r\n\r\n{\r\n  \"prefabName\": \"Item_HealthPotion\",\r\n  \"root\": {\r\n    \"id\": \"rootHealthPotion\",\r\n    \"name\": \"HealthPotion\"\r\n  },\r\n  \"steps\": [\r\n    { \"type\": \"create_object\", \"id\": \"rootHealthPotion\", \"name\": \"HealthPotion\" },\r\n    { \"type\": \"add_component\", \"target\": \"rootHealthPotion\", \"component\": \"Transform\" },\r\n    { \"type\": \"add_component\", \"target\": \"rootHealthPotion\", \"component\": \"Item\" },\r\n    \r\n    { \"type\": \"add_child\", \"id\": \"visual\", \"parent\": \"rootHealthPotion\", \"name\": \"Visual\" },\r\n    { \"type\": \"add_component\", \"target\": \"visual\", \"component\": \"Transform\" },\r\n    { \"type\": \"add_component\", \"target\": \"visual\", \"component\": \"MeshRenderer\" },\r\n    \r\n    { \"type\": \"add_child\", \"id\": \"collider\", \"parent\": \"rootHealthPotion\", \"name\": \"HitBox\" },\r\n    { \"type\": \"add_component\", \"target\": \"collider\", \"component\": \"BoxCollider\" },\r\n    { \"type\": \"set_property\", \"target\": \"collider\", \"component\": \"BoxCollider\", \"property\": \"size\", \"value\": \"1.0,1.0,1.0\" },\r\n    \r\n    { \"type\": \"add_child\", \"id\": \"art_branch_01\", \"parent\": \"rootHealthPotion\", \"name\": \"Branch_01\" },\r\n    { \"type\": \"add_component\", \"target\": \"art_branch_01\", \"component\": \"Transform\" },\r\n    { \"type\": \"add_component\", \"target\": \"art_branch_01\", \"component\": \"MeshRenderer\" },\r\n    { \"type\": \"set_property\", \"target\": \"art_branch_01\", \"component\": \"MeshRenderer\", \"property\": \"material\", \"value\": \"object:Branch_01\" }\r\n  ]\r\n}\r\n\r\n如果不是在新建prefab，你可以这样填写参数\"root\": {\r\n    \"id\": \"Adjustment\",\r\n    \"name\": \"Adjustment\"\r\n  },\r\n\r\nPrefabJson 命令格式说明\r\n\r\n1. create_object\r\n   • 用途：在 Prefab 内创建一个新的空 GameObject 或根对象\r\n\r\n   • 必填字段：\r\n\r\n       type: \"create_object\"\r\n       id: 唯一标识符，用于引用该对象\r\n       name: GameObject 名称\r\n   • 示例：\r\n\r\n       { \"type\": \"create_object\", \"id\": \"root\", \"name\": \"HealthPotion\" }\r\n\r\n2. add_child\r\n   • 用途：在父对象下添加一个子 GameObject，可以是空对象或实例化的美术 prefab\r\n\r\n   • 必填字段：\r\n\r\n       type: \"add_child\"\r\n       id: 唯一标识符，用于引用该对象\r\n       parent: 父对象的 id\r\n       name: 子对象名称\r\n   • 备注：\r\n\r\n       ◦ 如果 name 对应的美术资源 prefab，Executor 会实例化它\r\n\r\n       ◦ 空对象时直接创建 GameObject\r\n\r\n   • 示例：\r\n\r\n       { \"type\": \"add_child\", \"id\": \"visual\", \"parent\": \"root\", \"name\": \"Visual\" }\r\n\r\n3. add_component\r\n   • 用途：给指定对象添加组件（Unity 内置组件或自定义脚本）\r\n\r\n   • 必填字段：\r\n\r\n       type: \"add_component\"\r\n       target: 对象 id\r\n       component: 组件名称\r\n   • 备注：\r\n\r\n       ◦ Transform、Rigidbody、Collider 等内置组件必须显式添加\r\n\r\n       ◦ 自定义脚本组件也必须显式添加\r\n\r\n   • 示例：\r\n\r\n       { \"type\": \"add_component\", \"target\": \"visual\", \"component\": \"MeshRenderer\" }\r\n\r\n4. set_property\r\n   • 用途：设置组件字段或属性的值\r\n\r\n   • 必填字段：\r\n\r\n       type: \"set_property\"\r\n       target: 对象 id\r\n       component: 组件名称\r\n       property: 属性或字段名称\r\n       value: 设置值\r\n   • 备注：\r\n\r\n       ◦ 基本类型（int、float、bool、string）可直接写\r\n\r\n       ◦ Vector3 用数组 [x, y, z] 表示\r\n\r\n       ◦ 引用其他对象或组件（可以同一个prefab内部引用，也可以使用objectid引用外部prefab的信息）：\r\n\r\n           ▪ 对象引用: \"object:ID\"\r\n\r\n           ▪ 组件引用: \"component:ID:ComponentName\"\r\n\r\n       ◦ 美术资源 sprite 或材质通过 set_property 指定给对应组件字段\r\n\r\n   • 示例：\r\n\r\n       { \"type\": \"set_property\", \"target\": \"collider\", \"component\": \"BoxCollider\", \"property\": \"size\", \"value\": [1.0, 1.0, 1.0] }\r\n       { \"type\": \"set_property\", \"target\": \"art_branch_01\", \"component\": \"MeshRenderer\", \"property\": \"material\", \"value\": \"object:Branch_01\" }\r\n5. move\r\n\t• 调整已存在 GameObject 的父子关系\r\n\t• 不创建、不销毁、不修改组件或属性\r\n\t• 可以跨prefab移动\r\n \t{\"type\": \"move\",\"target\": \"enemySpawnerid\",\"parent\": \"Managersid\"}\r\n\r\n命令执行顺序：\r\n1. create_object\r\n2. add_child\r\n3. add_component\r\n4. set_property\r\n5. move\r\n注意事项：\r\n• 每个 prefab必须先新建一个gameobject作为root（除非是一个需要引用ui元素的物品，这种情况允许在canvas下新建物品）\r\n\r\n• 每个 GameObject 必须有唯一 id\r\n\r\n• 所有美术资源实例必须通过实例化生成，不直接使用原型\r\n\r\n• 所有引用必须使用 \"object:ID\" 或 \"component:ID:ComponentName\"\r\n\r\n• Transform 或其他 Unity 内置组件必须显式列出\r\n\r\n• JSON 严格按照执行顺序生成：create_object → add_child → add_component → set_property\r\n\r\n• 输出 JSON 中必须保持可解析结构【目标prefab】UIManager,UI管理器预制体，管理所有UI元素的显示和更新，监听游戏状态和数据变化，切换不同UI面板，更新生命值、金币、波次、时间等文本信息。包含UIManager脚本。";
        private static string MakePrefabDesignPrompt = "你是一个 Unity 游戏玩法架构助手。\r\n\r\n我将提供三部分信息：\r\n1. 游戏玩法说明（Gameplay Description）\r\n2. 系统与对象清单（System & Item List）\r\n3. 系统脚本设计文档（Script Responsibilities，仅用于参考）\r\n\r\n你的任务是：\r\n👉 在【不涉及美术、Transform 层级、组件组合、具体实现】的前提下，\r\n👉 从玩法和系统角度，列出“需要被设计为 Prefab 的游戏实体”。\r\n\r\n【Prefab 定义说明】\r\n- Prefab 是一个“可被实例化、复用、由系统驱动的责任实体”\r\n- 本阶段的 Prefab 是设计层概念，不是 Unity 结构设计\r\n\r\n【Prefab 责任类型（用于分类，不是游戏语义）】\r\n- Actor：可执行行为、参与玩法循环的实体\r\n- Interactive Object：主要响应交互的实体\r\n- Environment Entity：构成或影响游戏空间的实体\r\n- Trigger / Zone：基于条件或区域触发逻辑的实体\r\n- Spawner / Emitter：负责生成其他实体的实体\r\n- Logic Anchor：存在于场景中以承载逻辑的实体\r\n\r\n【输出格式】\r\n对每一个 Prefab，请使用以下格式：\r\n\r\nPrefab Name:\r\n- Responsibility Type:\r\n- Gameplay Responsibility:\r\n- Driven By Systems:\r\n- Spawns Other Prefabs:\r\n- Multiple Instances:\r\n- State Holder:\r\n- Notes:\r\n\r\nnote里面说明\r\n这个prefab有哪些脚本，脚本中哪些字段需要运行前赋值\r\n如果这个字段需要引用gameobject，或者component\r\n说明这个component/object应该存在于prefab的内部，还是说应该存在于其他prefab\r\n\r\n\r\n【重要约束】\r\n- 不要讨论美术、模型、动画\r\n- 不要设计层级结构或组件\r\n- 不要写代码\r\n- 不要假设未在玩法说明中出现的系统\r\n\r\n---";
        private static string gameSysetmPrompt = "你是一个资深unity游戏设计助手。你将根据用户提供的游戏玩法说明，生成详细的 系统和对象清单，用于支持游戏开发。  \r\n要求：\r\n1. 只关注玩法与功能，不涉及代码实现或美术风格。  \r\n2. 输出内容分为两个部分：\r\n   a. 系统（Core Systems）：列出游戏需要的核心逻辑模块，每个模块给出简短功能描述。  \r\n   b. 对象（Items / Game Objects）：列出游戏中需要的实体，每个实体说明功能或作用。  \r\n4. 输出清晰易读，方便用户理解和确认。\r\n5. 通过抽象程度给核心系统分层，保证强相关的系统在同一层。越不直接与玩家操作相连，或者在增量开发中可以放在后面开发的系统越抽象\r\n6. 这个设计后面会用来设计开发计划（开发模式为增量开发），从最小可行产品逐渐拓展到整个游戏\r\n7. 避免过度设计和制作没有提到的系统\r\n输出示例格式：\r\n\r\n---\r\n核心系统（Core Systems）：\r\n抽象程度0：\r\n1. 玩家控制系统：控制角色移动、跳跃、攻击等\r\n3. 敌人AI系统：控制敌人的行为和攻击模式\r\n抽象程度1：\r\n1. 血量系统\r\n2. 掉落物系统\r\n抽象程度3：\r\n\r\n\r\n游戏对象（Items / Game Objects）：\r\n1. 玩家角色：可移动、跳跃、攻击，拥有血量\r\n2. 敌人：攻击玩家，拥有血量\r\n3. 道具：可以被玩家收集（金币、宝石等）\r\n4. 平台：供玩家跳跃使用\r\n5. 终点/目标：触发关卡完成\r\n6. [其他对象，如果需要可提问]\r\n\r\n";
        private static string developmentPlanDescriptionPrompt = "你是一个资深的游戏技术策划 / 架构设计助手。\r\n\r\n\r\n\r\n\r\n我会给你：\r\n\r\n- 游戏的玩法说明\r\n\r\n- 已经整理好的【核心系统分层描述】，每个系统都标明了抽象程度\r\n\r\n\r\n\r\n\r\n你的任务不是写代码，而是**规划一个合理的开发步骤（Development Stages）**。\r\n\r\n\r\n\r\n\r\n### 总体目标\r\n\r\n请用【自然语言】描述一个**从最小可运行版本开始，逐步扩展到完整游戏的开发路线图**，注意，并不是做出可玩版本，目的是搭建程序和其他资源，而不是玩法验证。\r\n\r\n\r\n\r\n\r\n### 强制规则（非常重要）：\r\n\r\n1. **从具体到抽象**  \r\n\r\n   - 先实现最低抽象程度的系统  \r\n\r\n   - 后面的阶段只能建立在前一阶段已经“能跑”的基础上\r\n\r\n\r\n\r\n\r\n2. **每一个阶段都必须是一个“可运行版本”**\r\n\r\n   - 明确说明：这个阶段的版本玩家能做什么\r\n\r\n   - 明确说明：这个阶段主要是为了验证哪些系统是否工作正常\r\n\r\n\r\n\r\n\r\n3. **禁止跨抽象程度**\r\n\r\n   - 一个阶段只能使用【同一抽象程度】或【已完成的更低抽象程度】的系统\r\n\r\n   - 不能提前引入更高层的流程控制、UI 或全局管理\r\n\r\n\r\n\r\n\r\n4. **阶段数量适中**\r\n\r\n   - 不追求一步一个系统\r\n\r\n   - 如果同一抽象层的系统强相关，可以自然地合并在一个阶段中\r\n\r\n\r\n\r\n\r\n5. **只用自然语言**\r\n\r\n   - 不要输出 JSON\r\n\r\n   - 不要使用编号以外的结构化格式\r\n\r\n   - 不要写代码\r\n\r\n   - 不要讨论美术或实现细节\r\n\r\n\r\n\r\n\r\n6. **不用考虑美术，专注于玩法**\r\n\r\n   - 这个项目使用预制美术，后面开发的时候会直接使用现成美术资源\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n---\r\n\r\n\r\n\r\n\r\n### 输出格式要求：\r\n\r\n\r\n\r\n\r\n请按以下结构输出：\r\n\r\n\r\n\r\n\r\n开发阶段 1：\r\n\r\n- 阶段目标（用一句话概括这个版本“存在的意义”）\r\n\r\n- 这个版本玩家可以做什么\r\n\r\n- 本阶段重点验证的系统（用自然语言描述，不要简单列系统名）\r\n\r\n- 为什么这些系统必须在这个阶段一起完成\r\n\r\n\r\n\r\n\r\n开发阶段 2：\r\n\r\n（同上）\r\n\r\n\r\n\r\n\r\n开发阶段 3：\r\n\r\n（同上）\r\n\r\n\r\n\r\n\r\n……\r\n\r\n\r\n\r\n\r\n---\r\n\r\n\r\n\r\n\r\n现在，请根据下面提供的【玩法说明】和【核心系统分层描述】，生成完整的开发阶段说明。";
        private static string MakeDevelopmentPlanJson = "你是一个结构化整理助手。\r\n\r\n我将提供一组【已经确认的游戏开发阶段的自然语言描述】。\r\n这些阶段已经过设计，不需要你重新规划或优化。\r\n\r\n你的任务是：\r\n**仅将这些自然语言阶段，转换为一个 JSON 结构，用于后续自动化处理。**\r\n\r\n⚠️ 严格规则（非常重要）：\r\n1. **不要新增、删除或合并阶段**\r\n2. **不要引入新的系统、概念或抽象**\r\n3. **不要跨阶段重组内容**\r\n4. **不要评价设计是否合理**\r\n5. 你只能做“整理”和“转写”\r\n\r\n---\r\n\r\n### JSON 结构要求（必须严格遵守）：\r\n\r\n请使用以下 JSON 格式（字段名不可更改）：\r\n\r\n{\r\n  \"stages\": [\r\n    {\r\n      \"title\": \"Stage 1 的简短标题\",\r\n      \"description\": \"完整描述该阶段的可运行版本目标。需要清楚说明：这个版本玩家能做什么，以及这个阶段主要是为了验证哪些系统是否正常工作。\"\r\n    },\r\n    {\r\n      \"title\": \"Stage 2 的简短标题\",\r\n      \"description\": \"完整描述该阶段的可运行版本目标。需要清楚说明：这个版本玩家能做什么，以及这个阶段主要是为了验证哪些系统是否正常工作。\"\r\n    }\r\n  ]\r\n}\r\n\r\n\r\n- `title`：为该阶段提炼一个**简短、概括性的标题**\r\n- `description`：\r\n  - 必须完整覆盖该阶段的目标\r\n  - 必须描述这是一个**可运行版本**\r\n  - 不要拆成列表，使用一段完整的自然语言描述\r\n\r\n---\r\n\r\n### 重要内容约束：\r\n- 每个 stage 描述的内容 **只能来自该阶段的自然语言说明**\r\n- 不允许提前引入后续阶段的系统或目标\r\n- 描述应保持“从具体到抽象”的原始顺序，不做提升或泛化\r\n\r\n---\r\n\r\n现在，请根据下面提供的【开发阶段自然语言描述】，生成最终的 JSON。\r\n\r\n【开发阶段自然语言描述】：";
        private static string MakeCodeDesign = "你是一个资深 Unity C# 游戏开发 AI 助手。\r\n\r\n\r\n\r\n\r\n我将提供以下信息：\r\n\r\n1. 游戏玩法描述\r\n\r\n2. 系统清单\r\n\r\n3. 开发计划描述（每个阶段自然语言说明）\r\n\r\n4. 当前阶段（Stage X）\r\n\r\n5. 游戏对象清单及属性/接口\r\n\r\n6. Unity 环境信息（版本、物理/输入系统、C# 规范）\r\n\r\n\r\n\r\n\r\n你的任务：\r\n\r\n1. **分析当前阶段需要实现的系统**，确定需要哪些脚本（Script）来实现这些系统，确保系统模块化、低耦合、便于未来扩展，不需要的脚本不需要输出。\r\n\r\n2. **列出每个脚本的名称**（可以使用示意名称，如 PlayerMovement.cs、EnemyAI.cs）。\r\n\r\n3. **说明每个脚本的职责**（Script负责的功能，不需要具体代码）。\r\n\r\n4. **说明脚本之间的交互关系**，包括调用、监听、依赖或事件订阅。\r\n\r\n5. **考虑未来扩展**：\r\n\r\n   - 哪些脚本可能在后续阶段需要更改\r\n\r\n   - 给出避免未来返工的策略（例如通过接口、事件或 Hook 预留）\r\n\r\n6. 聚焦于**代码功能和关系**，不要生成实现代码。\r\n\r\n7. 输出清晰可读，方便开发者理解整个系统架构和脚本职责。\r\n\r\n8. **输出格式**：\r\n\r\n   - 文本格式，不使用 Markdown\r\n\r\n   - 脚本总览一定要精确，列出所有脚本的文件名，格式如下：\r\n\r\n\r\n\r\n\r\n注意：\r\n• 禁止使用 FindObjectWithTag / GameObject.Find 等通过字符串查找对象的方式\r\n• 不需要写出可选脚本\r\n\r\n思考\r\n\r\n这个脚本能不能被unity自带的功能（比如cinemachine实现）而不需要单独写\r\n\r\n这个脚本和unity的物理系统有没有交互，和其他unity系统有没有交互\r\n\r\n思考如何避免循环引用，以及列出每个脚本的依赖关系\r\n\r\n这个脚本应该被什么物品挂载？\r\n\r\n这个是原型测试阶段，不需要做过于复杂的结构也不需要考虑优化\r\n\r\n有没有哪些脚本不用拆分，来避免脚本系统过于臃肿\r\n\r\n不需要考虑音频\r\n\r\n如果相机不需要脚本，就不用写出那个脚本\r\n\r\n\r\n\r\n\r\n输出格式：\r\n\r\n脚本总览：\r\n\r\n1. xxx\r\n详细内容\r\n\r\n2. xxx\r\n详细内容\r\n\r\n3. xxx\r\n\r\n\r\n";
        private static string MakeCodeGenerationBatchJson = "你是一个 Unity C# 开发助手。 我会提供以下信息： 1. 游戏系统设计和每个脚本的职责说明。 2. 需要生成的脚本列表（例如 PlayerInput.cs, PlayerMovement.cs, ...）。 你的任务： 1. 根据依赖关系和系统逻辑，把这些脚本拆分成若干批次，每批次包含若干脚本： - 先生成被依赖的脚本，再生成依赖它的脚本 - 同一批次内部也按依赖关系排序 - 批次数量可以根据依赖关系自动调整，不固定 2. 输出结构化的 JSON，格式如下（严格遵守，便于 Unity JsonUtility 解析）： { \"batches\": [ { \"batchIndex\": 0, \"scripts\": [ \"a.cs\", \"b.cs\", \"c.cs\" ] }, { \"batchIndex\": 1, \"scripts\": [ \"d.cs\", \"e.cs\", \"f.cs\", \"g.cs\" ] }, { \"batchIndex\": 2, \"scripts\": [ \"h.cs\", \"i.cs\", \"j.cs\", \"k.cs\" ] }, { \"batchIndex\": 3, \"scripts\": [ \"l.cs\", \"m.cs\", \"n.cs\" ] }, { \"batchIndex\": 4, \"scripts\": [ \"o.cs\", \"p.cs\" ] } ] } 3. JSON 中： - \"batchIndex\" 从 0 开始，每个 batch 包含 scripts 列表 - scripts 列表按生成顺序排列 4. 只输出 JSON，不要任何额外文字或解释。 5. 保证 JSON 可以被 Unity JsonUtility 直接解析。";
        private static string UnityPackages = "Cinemachine,TextMeshPro ,unity 物理与其他自带系统, Dotween,unity旧输入系统，你可以使用unity里面的功能来实现碰撞反应，镜头跟随等一些内容，而不需要单独写脚本";
        private static string GenerateWriteCodeCommand = "你是一个Unity C#开发助手。\r\n我会提供以下信息：\r\n1. 系统描述和脚本职责。\r\n2. 需要生成的脚本列表，例如：\"要生成的脚本为：PlayerController, EnemyAI, GameManager\"。\r\n\r\n你的任务：\r\n1. 为每个脚本生成一个完整的C#脚本，包含：\r\n   • public class [脚本名]\r\n\r\n   • 根据系统描述和脚本职责，实现类内部逻辑和方法\r\n\r\n   • 为每个类生成 /// <summary> 注释\r\n\r\n   • 为每个变量生成 /// <var> 注释\r\n\r\n   • 为每个方法生成 /// <method> 注释\r\n\r\n   • 为每个参数生成 /// <param> 注释\r\n\r\n   • 为每个返回值生成 /// <returns> 注释（如有）\r\n\r\n   • 其他位置不要生成此类标签注释\r\n\r\n2. 将每个脚本生成一条 Unity命令：\r\n   • 命令格式：\r\n\r\nwrite_code: file_path={SCRIPT_PATH}/{脚本名}.cs, content=完整脚本内容\r\n\r\n3. 所有脚本都放在指定的 Scripts 目录下。\r\n4. 直接输出命令列表，不要额外文字解释。\r\n5. 脚本逻辑尽量完整，能体现职责描述里的功能。类标注要写明这个脚本是否是singleton，目标物品是什么\r\n注意事项：\r\n• 仅对换行使用 \"\\\\n\"，缩进使用 \"\\\\t\"，除 \\\\n、\\\\t 以外不得额外转义任何 C# 语法符号\r\n\r\n• 保留所有 C# 运算符和语法符号原样：==, !=, >, <, &&, ||, +=, -=, *, /, %, $, {} ,\"，()等\r\n\r\n• 每个 write_code 命令输出为单行\r\n\r\n• 注释必须严格使用 <summary>/<var>/<method>/<param>/<returns>\r\n\r\n• 不要在其他位置生成注释\r\n\r\n•  需要在 Inspector 里面赋值的，与一般使用 [SerializeField] private 的变量都写为 public 的，不要出现 [SerializeField] private\r\n\r\n•  禁止使用 FindObjectWithTag / GameObject.Find 等通过字符串查找对象的方式\r\n\r\n\r\n示例命令（用于说明转义规则与 C# 语法保留方式）：\r\nwrite_code: file_path={SCRIPT_PATH}/ExampleScript.cs, content=using UnityEngine;\\n\\n/// <summary>\\n/// 示例脚本，用于展示运算符与转义规则\\n/// </summary>\\npublic class ExampleScript : MonoBehaviour\\n{\\n\\t/// <var>\\n\\t/// 示例数值\\n\\t/// </var>\\n\\tpublic int value = 10;\\n\\n\\t/// <method>\\n\\t/// 计算并输出结果\\n\\t/// </method>\\n\\t/// <param name=\"delta\">变化值</param>\\n\\t/// <returns>计算后的结果</returns>\\n\\tpublic int Calculate(int delta)\\n\\t{\\n\\t\\tif(value != 0 && delta > 0)\\n\\t\\t{\\n\\t\\t\\tvalue += delta * 2;\\n\\t\\t}\\n\\t\\treturn value;\\n\\t}\\n}\r\n\r\n";
        private static string GenerateSceneMakingDescription = "你是 Unity 场景设计助手。\r\n\r\n任务：基于现有美术资源和 Prefab，生成一个完整场景的构建步骤。  \r\n\r\n要求：\r\n1. 输出自然语言描述，不需要 JSON 或具体 Unity 操作。\r\n2. 每个步骤用固定格式编号，格式为：Step N: 描述\r\n   • N 从 1 开始递增。\r\n\r\n   • 描述包含生成场景逻辑、资源使用、对象放置思路。\r\n\r\n3. AI 自动考虑：\r\n\t◦ 摄像机位置。\r\n\r\n   • 基础地形先行，保证可行走区域。\r\n\r\n   • 静态装饰（树、石头、道具）随机或规则摆放。\r\n\r\n   • 非 Singleton Prefab（敌人、NPC、机关）放置位置。\r\n\r\n   • 玩家出生点、事件触发点和主要路径。\r\n\r\n   • 可使用美术资源列表判断哪些资源适合做地形或装饰。\r\n\r\n4. 输出的每个步骤必须可以用正则 `Step \\d+: (.+)` 提取。\r\n5. 步骤数量不限，但应覆盖场景主要逻辑。\r\n6. 特殊物品id：DefaultSceneStaticItemRoot，DefaultSceneDynamicItemRoot（对应gameobject默认已经存在于场景中（0，0，0）的位置）\r\n7. 保证所有步骤结束后，所有单例物品都是DefaultSceneStaticItemRoot和DefaultSceneDynamicItemRoot的childobject\r\n8. 专注于玩法开发，不需要过度装饰\r\n\r\n示例输出（仅演示格式）：\r\nStep 1: ...\r\nStep 2: ...\r\nStep 3: ...\r\nStep 4: ...\r\nStep 5: ...\r\nStep 6: ...\r\n";
    }
}