using AiHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Networking;




public class TextCommandExecutor
{
    public static GameObject ExecuteGameobjectCommand(string prefabCommand)
    {
       return PrefabJsonExecutor.Instance.ExecutePrefabJsonFromText(prefabCommand);

    }
    public static void ExecuteWriteCodeCommands(string commandText)
    {
        Debug.Log("final command:" + commandText);
        if (string.IsNullOrEmpty(commandText))
        {
            Debug.LogWarning("命令文本为空！");
            return;
        }

        // 支持多条命令，每条命令换行分隔
        string[] lines = commandText.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            // 简单解析命令
            if (line.StartsWith("write_code:"))
            {
                ExecuteWriteCode(line);
            }
            else
            {
                Debug.LogWarning("未知命令：" + line);
            }
        }
        AssetDatabase.Refresh();
    }

    private static void ExecuteWriteCode(string line)
    {
        Debug.Log(line);
        string argsPart = line.Substring("write_code:".Length).Trim();

        int contentIndex = argsPart.IndexOf("content=");
        if (contentIndex < 0)
        {
            Debug.LogError("命令格式错误，缺少 content 参数！");
            return;
        }

        string filePathPart = argsPart.Substring(0, contentIndex).Trim().TrimEnd(',');
        string contentPart = argsPart.Substring(contentIndex + "content=".Length).Trim();

        string filePath = null;
        string content = null;

        if (filePathPart.StartsWith("file_path="))
        {
            filePath = filePathPart.Substring("file_path=".Length).Trim();
        }

        content = contentPart;

        // 去掉外层引号（如果有）
        if (content.StartsWith("\"") && content.EndsWith("\"") && content.Length >= 2)
        {
            content = content.Substring(1, content.Length - 2);
        }

        if (string.IsNullOrEmpty(filePath) || content == null)
        {
            Debug.LogError($"命令格式错误，缺少文件路径或内容！{filePath}  all command:{line}");
            return;
        }

        Debug.Log($"[TextCommandExecutor] 即将写入文件：{filePath}\n内容预览：\n{content}");



        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(filePath, content.Replace("\\n", "\n").Replace("\\t", "\t"));
        Debug.Log("已写入文件：" + filePath);
    }


}
public class readFileUtility
{
    public static string GetAllFileNames(string folderPath)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath);

        if (!Directory.Exists(fullPath))
        {
            Debug.LogError($"目录不存在: {fullPath}");
            return "";
        }

        string[] files = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);
        var returnString = "";
        foreach (var file in files)
        {
            returnString += (Path.GetFileName(file) + ", "); // ✅ 只取文件名
        }

        return returnString;
    }

    public static (StringBuilder, Dictionary<string, string>) GenerateAllCodeDescription(string ScriptPath)
    {
        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), ScriptPath)))
        {
            Debug.LogError($"尝试访问的目录不存在: {ScriptPath}");
            return (null, null);
        }

        // 获取目录下的所有.cs文件
        string[] allScripts = Directory.GetFiles(
            Path.Combine(Directory.GetCurrentDirectory(), ScriptPath),
            "*.cs",
            SearchOption.AllDirectories
        );

        // ===== 2. 生成所有脚本的简介 =====
        StringBuilder allScriptsIntro = new StringBuilder();
        Dictionary<string, string> scriptPathToIntro = new Dictionary<string, string>();

        foreach (var scriptFullPath in allScripts)
        {
            try
            {
                string code = File.ReadAllText(scriptFullPath);
                string relativePath = scriptFullPath.Replace(Directory.GetCurrentDirectory(), "").Replace("\\", "/").TrimStart('/');

                // 使用正则提取脚本简介
                string intro = ExtractScriptIntro(code, relativePath);

                allScriptsIntro.AppendLine(intro);
                allScriptsIntro.AppendLine("---");

                scriptPathToIntro[relativePath] = intro;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"读取脚本失败: {scriptFullPath}, 错误: {ex.Message}");
            }
        }
        return (allScriptsIntro, scriptPathToIntro);
    }

    private static string ExtractScriptIntro(string code, string relativePath)
    {
        StringBuilder intro = new StringBuilder();
        intro.AppendLine($"脚本: {relativePath}");
        intro.AppendLine(readFileUtility.GenerateDescription(code));
        return intro.ToString();
    }
    public static string ReadFile(string scriptFullPath)
    {
        string code = File.ReadAllText(scriptFullPath);
        return code;
    }
    public static string GenerateDescription(string classContent)
    {
        StringBuilder result = new StringBuilder();
        string[] lines = classContent.Split('\n');
        int i = 0;
        bool inClassSummary = false;
        bool inMethodSummary = false;
        StringBuilder pendingClassSummary = new StringBuilder();
        StringBuilder pendingMethodSummary = new StringBuilder();
        Dictionary<string, string> pendingParamDesc = new Dictionary<string, string>();

        while (i < lines.Length)
        {
            string line = lines[i].Trim();

            // ===== 类注释 =====
            if (line.StartsWith("/// <summary>"))
            {
                inClassSummary = true;
                pendingClassSummary.Clear();
                i++;
                continue;
            }

            if (inClassSummary && line.StartsWith("/// </summary>"))
            {
                inClassSummary = false;
                // 检查接下来的行是否是类定义
                int j = i + 1;
                while (j < lines.Length && string.IsNullOrWhiteSpace(lines[j]))
                {
                    j++;
                }
                if (j < lines.Length && (lines[j].Trim().StartsWith("public class") ||
                    lines[j].Trim().StartsWith("private class") ||
                    lines[j].Trim().StartsWith("internal class") ||
                    lines[j].Trim().StartsWith("class")))
                {
                    result.AppendLine($"类: {pendingClassSummary.ToString().Trim()}");
                }
                i++;
                continue;
            }

            if (inClassSummary)
            {
                pendingClassSummary.Append(line.Replace("///", "").Trim() + " ");
                i++;
                continue;
            }

            // ===== 变量注释 =====
            if (line.StartsWith("/// <var>"))
            {
                StringBuilder varDescription = new StringBuilder();

                // 提取<var>标签内的内容，可能跨越多行
                if (line.Contains("</var>"))
                {
                    // 单行情况
                    varDescription.Append(line.Replace("/// <var>", "").Replace("</var>", "").Trim());
                }
                else
                {
                    // 多行情况
                    varDescription.Append(line.Replace("/// <var>", "").Trim());
                    i++;
                    while (i < lines.Length && !lines[i].Trim().Contains("</var>"))
                    {
                        varDescription.Append(lines[i].Replace("///", "").Trim());
                        i++;
                    }
                    if (i < lines.Length && lines[i].Trim().Contains("</var>"))
                    {
                        varDescription.Append(lines[i].Replace("///", "").Replace("</var>", "").Trim());
                    }
                }

                string description = varDescription.ToString().Trim();

                // 查找变量定义行
                int j = i + 1;
                while (j < lines.Length && (string.IsNullOrWhiteSpace(lines[j]) || lines[j].Trim().StartsWith("///")))
                {
                    j++;
                }

                if (j < lines.Length)
                {
                    string varLine = lines[j].Trim();

                    // 匹配 public、private、protected 变量声明
                    // 包括处理数组类型，如 GameObject[]
                    var match = Regex.Match(
                        varLine,
                        @"^\s*(public|private|protected|internal)\s+(static\s+)?([\w\.<>\[\]]+)\s+(\w+)(?:\s*=\s*.+)?\s*;"
                    );

                    if (match.Success)
                    {
                        string accessModifier = match.Groups[1].Value;
                        string type = match.Groups[3].Value;
                        string name = match.Groups[4].Value;

                        // 只输出 public 和 internal 变量
                        if (accessModifier == "public" || accessModifier == "internal")
                        {
                            if (!result.ToString().Contains("变量:"))
                                result.AppendLine("\n变量:");

                            result.AppendLine($"  {name}（{type}）: {description}");
                        }
                    }
                }
            }

            // ===== 方法注释 =====
            if (line.StartsWith("/// <method>") || (line.StartsWith("/// <summary>") && !inClassSummary))
            {
                inMethodSummary = true;
                pendingMethodSummary.Clear();
                pendingParamDesc.Clear();

                // 提取注释内容
                if (line.Contains("</method>"))
                {
                    pendingMethodSummary.Append(line.Replace("/// <method>", "").Replace("</method>", "").Trim());
                    inMethodSummary = false;
                }
                else if (line.Contains("</summary>"))
                {
                    pendingMethodSummary.Append(line.Replace("/// <summary>", "").Replace("</summary>", "").Trim());
                    inMethodSummary = false;
                }
                else
                {
                    if (line.StartsWith("/// <method>"))
                        pendingMethodSummary.Append(line.Replace("/// <method>", "").Trim() + " ");
                    else if (line.StartsWith("/// <summary>"))
                        pendingMethodSummary.Append(line.Replace("/// <summary>", "").Trim() + " ");
                }

                i++;
                continue;
            }

            if (inMethodSummary)
            {
                if (line.StartsWith("/// </method>") || line.StartsWith("/// </summary>"))
                {
                    inMethodSummary = false;
                    i++;
                    continue;
                }

                pendingMethodSummary.Append(line.Replace("///", "").Trim() + " ");
                i++;
                continue;
            }

            // ===== 方法参数注释 =====
            if (line.StartsWith("/// <param"))
            {
                // 处理参数注释
                StringBuilder paramComment = new StringBuilder();
                string paramName = "";

                var paramMatch = Regex.Match(line, @"name=""(\w+)"">(.+)");
                if (paramMatch.Success)
                {
                    paramName = paramMatch.Groups[1].Value;
                    string paramDesc = paramMatch.Groups[2].Value.Trim();
                    paramComment.Append(paramDesc);

                    if (!line.Contains("</param>"))
                    {
                        i++;
                        while (i < lines.Length && lines[i].Trim().StartsWith("///") && !lines[i].Trim().Contains("</param>"))
                        {
                            paramComment.Append(lines[i].Replace("///", "").Trim() + " ");
                            i++;
                        }
                        if (i < lines.Length && lines[i].Trim().Contains("</param>"))
                        {
                            paramComment.Append(lines[i].Replace("///", "").Replace("</param>", "").Trim());
                        }
                    }
                    else
                    {
                        paramComment.Clear();
                        paramComment.Append(line.Substring(line.IndexOf(">") + 1).Replace("</param>", "").Trim());
                    }

                    pendingParamDesc[paramName] = paramComment.ToString().Trim();
                }
            }

            // ===== 方法返回值注释 =====
            if (line.StartsWith("/// <returns>"))
            {
                // 跳过返回值注释，不处理
                i++;
                continue;
            }

            // ===== 检测方法定义 =====
            if (pendingMethodSummary.Length > 0)
            {
                // 检查是否是方法定义行
                if (!line.StartsWith("///") && !string.IsNullOrWhiteSpace(line) &&
                    (line.Contains("(") || line.Contains(" void ") || Regex.IsMatch(line, @"^\s*(public|private|protected|internal|static)\s+")))
                {
                    // 收集多行的方法定义
                    StringBuilder methodSignature = new StringBuilder();
                    int braceCount = 0;
                    int k = i;

                    // 构建完整的方法签名
                    while (k < lines.Length)
                    {
                        string methodLine = lines[k].Trim();

                        // 添加当前行
                        if (methodSignature.Length > 0)
                            methodSignature.Append(" ");
                        methodSignature.Append(methodLine);

                        // 计算括号
                        foreach (char c in methodLine)
                        {
                            if (c == '(') braceCount++;
                            if (c == ')') braceCount--;
                        }

                        // 找到方法体开始或分号结束
                        if (braceCount == 0 && (methodLine.Contains("{") || methodLine.Contains(";")))
                        {
                            // 提取方法签名（去掉方法体）
                            string fullSignature = methodSignature.ToString();
                            int endIndex = Math.Min(
                                fullSignature.IndexOf('{') >= 0 ? fullSignature.IndexOf('{') : int.MaxValue,
                                fullSignature.IndexOf(';') >= 0 ? fullSignature.IndexOf(';') : int.MaxValue
                            );

                            if (endIndex < int.MaxValue)
                            {
                                fullSignature = fullSignature.Substring(0, endIndex).Trim();
                            }

                            // 匹配方法签名
                            var methodMatch = Regex.Match(
                                fullSignature,
                                @"^\s*(public|private|protected|internal|static\s+)?\s*([\w\<\>\.\[\]]+\s+)?(\w+)\s*\(([^)]*)\)"
                            );

                            if (methodMatch.Success)
                            {
                                string accessModifier = methodMatch.Groups[1].Value.Trim();
                                string returnType = methodMatch.Groups[2].Value.Trim();
                                string methodName = methodMatch.Groups[3].Value;
                                string paramList = methodMatch.Groups[4].Value.Trim();

                                // 只处理 public 和 internal 方法
                                if (accessModifier == "public" || accessModifier == "internal" ||
                                    (string.IsNullOrEmpty(accessModifier) && returnType == "public") ||
                                    (accessModifier.StartsWith("public") || accessModifier.StartsWith("internal")))
                                {
                                    if (!result.ToString().Contains("方法:"))
                                        result.AppendLine("\n方法:");

                                    // 构造方法签名
                                    StringBuilder methodLineOutput = new StringBuilder();
                                    methodLineOutput.Append($"  {methodName}(");

                                    List<string> paramOutput = new List<string>();

                                    if (!string.IsNullOrEmpty(paramList))
                                    {
                                        // 处理参数列表
                                        List<string> parameters = new List<string>();
                                        int genericDepth = 0;
                                        StringBuilder currentParam = new StringBuilder();

                                        foreach (char c in paramList)
                                        {
                                            if (c == '<') genericDepth++;
                                            else if (c == '>') genericDepth--;

                                            if (c == ',' && genericDepth == 0)
                                            {
                                                parameters.Add(currentParam.ToString().Trim());
                                                currentParam.Clear();
                                            }
                                            else
                                            {
                                                currentParam.Append(c);
                                            }
                                        }
                                        if (currentParam.Length > 0)
                                            parameters.Add(currentParam.ToString().Trim());

                                        foreach (var p in parameters)
                                        {
                                            if (!string.IsNullOrWhiteSpace(p))
                                            {
                                                // 匹配参数类型和名称
                                                var pMatch = Regex.Match(p, @"([\w\.<>\[\]\s]+)\s+(\w+)");
                                                if (pMatch.Success)
                                                {
                                                    string pType = pMatch.Groups[1].Value.Trim();
                                                    string pName = pMatch.Groups[2].Value;
                                                    paramOutput.Add($"{pType} {pName}");
                                                }
                                                else
                                                {
                                                    paramOutput.Add(p.Trim());
                                                }
                                            }
                                        }
                                    }

                                    methodLineOutput.Append(string.Join(", ", paramOutput));
                                    methodLineOutput.Append("): ");
                                    methodLineOutput.Append(pendingMethodSummary.ToString().Trim());

                                    // 添加参数说明
                                    if (pendingParamDesc.Count > 0)
                                    {
                                        methodLineOutput.Append(" (");
                                        bool firstParam = true;
                                        foreach (var kv in pendingParamDesc)
                                        {
                                            if (!firstParam) methodLineOutput.Append(", ");
                                            methodLineOutput.Append($"{kv.Key}: {kv.Value}");
                                            firstParam = false;
                                        }
                                        methodLineOutput.Append(")");
                                    }

                                    result.AppendLine(methodLineOutput.ToString());
                                }

                                // 重置状态
                                pendingMethodSummary.Clear();
                                pendingParamDesc.Clear();
                                i = k; // 跳过已处理的行
                            }
                            break;
                        }
                        k++;
                    }
                }
            }

            i++;
        }

        return result.ToString().Trim();
    }
}


[InitializeOnLoad]
public static class CompilationErrorHandler
{
    static CompilationErrorHandler()
    {
        CompilationPipeline.assemblyCompilationFinished += OnCompilationFinished;
    }

    private static void OnCompilationFinished(string assemblyPath, CompilerMessage[] compilerMessages)
    {
        try
        {
            var errors = compilerMessages.Where(m => m.type == CompilerMessageType.Error).ToArray();
            if (errors.Length == 0) return;

            Debug.LogError("Compilation failed! Generating AI Analysis Prompt...");

            // 收集错误文件和类型
            HashSet<string> errorFiles = new HashSet<string>();
            HashSet<string> suspectedTypes = new HashSet<string>();
            foreach (var e in errors)
            {
                if (!string.IsNullOrEmpty(e.file))
                    errorFiles.Add(e.file.Replace("\\", "/"));
                string typeName = TryExtractTypeName(e.message);
                if (!string.IsNullOrEmpty(typeName))
                    suspectedTypes.Add(typeName);
            }

            StringBuilder errorSummary = new StringBuilder();
            foreach (var e in errors.Take(5))
                errorSummary.AppendLine($"- {e.file}:{e.line} - {e.message}");

            // ===== 第一步：分析窗口 prompt =====
            StringBuilder firstPrompt = new StringBuilder();
            firstPrompt.AppendLine("你是 Unity C# 编译错误分析助手。");
            firstPrompt.AppendLine("请分析编译错误，列出需要查看完整代码的脚本（NEED_SCRIPTS）并做分析（ANALYSIS）。");
            firstPrompt.AppendLine();
            firstPrompt.AppendLine("===== 错误摘要 =====");
            firstPrompt.AppendLine(errorSummary.ToString());
            if (suspectedTypes.Count > 0)
            {
                firstPrompt.AppendLine();
                firstPrompt.AppendLine("===== 疑似类型 =====");
                firstPrompt.AppendLine(string.Join(", ", suspectedTypes));
            }
            firstPrompt.AppendLine();
            firstPrompt.AppendLine("请严格按格式输出：");
            firstPrompt.AppendLine("NEED_SCRIPTS_START");
            firstPrompt.AppendLine("Assets/Scripts/Script1.cs");
            firstPrompt.AppendLine("Assets/Scripts/Script2.cs");
            firstPrompt.AppendLine("NEED_SCRIPTS_END");
            firstPrompt.AppendLine("ANALYSIS_START");
            firstPrompt.AppendLine("你的分析说明...");
            firstPrompt.AppendLine("ANALYSIS_END");

            var analysisWindow = requestAiResponse.Open(firstPrompt.ToString());


            analysisWindow.OnConfirmed += aiAnalysisResponse =>
            {
                analysisWindow.Close();

                // ===== 第二步：生成写代码窗口 prompt =====
                string secondPrompt = BuildAIDebugPrompt(aiAnalysisResponse);
                var debugWindow = requestAiResponse.Open(secondPrompt);

                debugWindow.OnConfirmed += writeCodeCommands =>
                {
                    debugWindow.Close();
                    if (!string.IsNullOrEmpty(writeCodeCommands))
                    {
                        TextCommandExecutor.ExecuteWriteCodeCommands(writeCodeCommands);
                        Debug.Log("[CompilationErrorHandler] 已执行 write_code 指令");
                    }
                };
            };
        }
        catch (Exception ex)
        {
            OpenRequestWindowWithLog($"CompilationErrorHandler 出现异常：{ex.Message}\n{ex.StackTrace}");
        }
    }

    private static string BuildAIDebugPrompt(string aiAnalysisResponse)
    {
        // ===== 解析 NEED_SCRIPTS =====
        List<string> scriptPaths = new List<string>();
        bool inNeedScripts = false;
        foreach (var line in aiAnalysisResponse.Split('\n'))
        {
            string trimmed = line.Trim();
            if (trimmed == "NEED_SCRIPTS_START") { inNeedScripts = true; continue; }
            if (trimmed == "NEED_SCRIPTS_END") { inNeedScripts = false; break; }
            if (inNeedScripts && !string.IsNullOrEmpty(trimmed) && File.Exists(trimmed))
                scriptPaths.Add(trimmed);
        }

        if (scriptPaths.Count == 0) return null;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("你是 Unity C# 编译错误修复助手。");
        sb.AppendLine("请根据以下脚本和分析生成 write_code 命令列表，每条命令严格单行输出，并遵循注释规范和示例格式。");
        sb.AppendLine();
        sb.AppendLine("===== AI分析内容 =====");
        sb.AppendLine(aiAnalysisResponse);
        sb.AppendLine();
        sb.AppendLine("===== 代码规范和示例 =====");
        sb.AppendLine("• public class [脚本名]");
        sb.AppendLine("• 根据系统描述和脚本职责，实现类内部逻辑和方法");
        sb.AppendLine("• 为每个类生成 /// <summary> 注释");
        sb.AppendLine("• 为每个变量生成 /// <var> 注释");
        sb.AppendLine("• 为每个方法生成 /// <method> 注释");
        sb.AppendLine("• 为每个参数生成 /// <param> 注释");
        sb.AppendLine("• 为每个返回值生成 /// <returns> 注释（如有）");
        sb.AppendLine("• 其他位置不要生成此类标签注释");
        sb.AppendLine("• 所有脚本生成 write_code 命令：");
        sb.AppendLine("  write_code: file_path={SCRIPT_PATH}/{脚本名}.cs, content=完整脚本内容");
        sb.AppendLine("• 直接输出命令列表，不要额外文字解释");
        sb.AppendLine("• 注意换行使用 \\n，缩进使用 \\t");
        sb.AppendLine("• 保留所有 C# 运算符原样");
        sb.AppendLine("• 示例命令：");
        sb.AppendLine("write_code: file_path={SCRIPT_PATH}/ExampleScript.cs, content=using UnityEngine;\\n\\n/// <summary>\\n/// 示例脚本，用于展示运算符与转义规则\\n/// </summary>\\npublic class ExampleScript : MonoBehaviour\\n{\\n\\t/// <var>\\n\\t/// 示例数值\\n\\t/// </var>\\n\\tpublic int value = 10;\\n\\n\\t/// <method>\\n\\t/// 计算并输出结果\\n\\t/// </method>\\n\\t/// <param name=\\\"delta\\\">变化值</param>\\n\\t/// <returns>计算后的结果</returns>\\n\\tpublic int Calculate(int delta)\\n\\t{\\n\\t\\tif(value != 0 && delta > 0)\\n\\t\\t{\\n\\t\\t\\tvalue += delta * 2;\\n\\t\\t}\\n\\t\\treturn value;\\n\\t}\\n}");

        sb.AppendLine();
        sb.AppendLine("===== 脚本完整内容 =====");

        foreach (var path in scriptPaths)
        {
            string content = ReadFile(path);
            sb.AppendLine($"// ===== {path} =====");
            sb.AppendLine(content.Replace("\r\n", "\\n").Replace("\t", "\\t"));
        }

        return sb.ToString();
    }

    private static string ReadFile(string scriptFullPath)
    {
        if (!File.Exists(scriptFullPath))
        {
            Debug.LogWarning($"ReadFile: 文件不存在 {scriptFullPath}");
            return "";
        }

        return File.ReadAllText(scriptFullPath);
    }

    private static void OpenRequestWindowWithLog(string logMessage)
    {
        var request = requestAiResponse.Open(logMessage);

        request.OnConfirmed += _ => request.Close();
        Debug.LogError(logMessage);
    }

    private static string TryExtractTypeName(string message)
    {
        int firstQuote = message.IndexOf('\'');
        if (firstQuote < 0) return null;
        int secondQuote = message.IndexOf('\'', firstQuote + 1);
        if (secondQuote < 0) return null;
        return message.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
    }
}
