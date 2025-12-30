using AiHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


public class LLMApiRelated
{
    // API 密钥和 URL
    private static string apiKey = "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";
    private static string apiUrl = "https://api.openai.com/v1/completions"; // OpenAI GPT-3 API 请求地址

    // 发起请求并返回 AIResponse 对象
    public static AIResponse SendLLMRequest(string prompt)
    {
        // 创建 AIResponse 实例
        AIResponse aiResponse = new AIResponse { requestId = Guid.NewGuid().ToString() };

        // 打印日志，表示请求已经发起
        Debug.Log($"[LLMApiRelated] Sending request for prompt: {prompt}");

        // 异步处理请求，内部使用协程（通过 UnityMainThreadDispatcher 来更新）
        LLMApiRelatedRequest(aiResponse, prompt);

        return aiResponse;
    }

    // 这是一个内部异步方法，用于处理 API 请求
    private static async void LLMApiRelatedRequest(AIResponse aiResponse, string prompt)
    {
        using (HttpClient client = new HttpClient())
        {
            // 设置请求头
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

            // 构造请求内容
            var requestData = new
            {
                model = "text-davinci-003",  // 使用的模型
                prompt = prompt,
                max_tokens = 150
            };

            string jsonData = JsonUtility.ToJson(requestData);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                // 发送 POST 请求
                Debug.Log("[LLMApiRelated] Sending POST request to OpenAI API...");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // 读取响应数据
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("[LLMApiRelated] API Response received successfully.");

                    // 解析响应并更新 AIResponse
                    var jsonResponse = JsonUtility.FromJson<OpenAIResponse>(responseBody);
                    string generatedText = jsonResponse.choices[0].text;

                    // 完成响应并触发事件
                    Debug.Log("[LLMApiRelated] Generated Text: " + generatedText);
                    aiResponse.MarkDone(generatedText);
                }
                else
                {
                    // 请求失败的处理
                    Debug.LogError($"[LLMApiRelated] Error: {response.StatusCode}");
                    aiResponse.MarkDone($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // 捕获并处理异常
                Debug.LogError($"[LLMApiRelated] Exception: {ex.Message}");
                aiResponse.MarkDone($"Error: {ex.Message}");
            }
        }
    }

    // 用于解析 OpenAI 返回的 JSON 响应
    [Serializable]
    public class OpenAIResponse
    {
        public Choice[] choices;

        [Serializable]
        public class Choice
        {
            public string text;
        }
    }
}
public class AIResponse
{
    // 响应的ID
    public string requestId;

    // AI生成的内容
    public string response = "waiting";

    // 请求是否完成的事件
    public event Action<string> OnDone;

    // 标记是否已完成（当设置为 true 时，触发事件）
    public void MarkDone(string newResponse)
    {
        response = newResponse;
        OnDone?.Invoke(response);  // 触发事件
    }
}
public class TextCommandExecutor
{



    
    public static void ExecuteCodeCommands(string commandText)
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
        public static string GenerateDescription(string classContent)
        {
            StringBuilder result = new StringBuilder();
            string[] lines = classContent.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                // ===== 类注释 =====
                if (line.StartsWith("/// <summary>"))
                {
                    i++;
                    StringBuilder classDesc = new StringBuilder();
                    while (i < lines.Length && !lines[i].Trim().StartsWith("/// </summary>"))
                    {
                        classDesc.Append(lines[i].Replace("///", "").Trim() + " ");
                        i++;
                    }
                    result.AppendLine($"类: {classDesc.ToString().Trim()}");
                }

                // ===== 变量注释 =====
                if (line.StartsWith("/// <var>"))
                {
                    string description = line.Replace("/// <var>", "").Replace("</var>", "").Trim();

                    if (i + 1 < lines.Length)
                    {
                        string nextLine = lines[i + 1].Trim();
                        var match = Regex.Match(
                            nextLine,
                            @"\b(public|private|protected)\s+([\w\.<>]+)\s+(\w+)"
                        );

                        if (match.Success)
                        {
                            string type = match.Groups[2].Value;
                            string name = match.Groups[3].Value;

                            if (!result.ToString().Contains("变量:"))
                                result.AppendLine("\n变量:");

                            result.AppendLine($"  {name}（{type}）: {description}");
                        }
                    }
                }

                // ===== 方法注释 =====
                if (line.StartsWith("/// <method>"))
                {
                    string description = line.Replace("/// <method>", "").Replace("</method>", "").Trim();
                    Dictionary<string, string> pendingParamDesc = new Dictionary<string, string>();

                    // 收集参数注释
                    int j = i + 1;
                    while (j < lines.Length && lines[j].Trim().StartsWith("///"))
                    {
                        string commentLine = lines[j].Trim();
                        if (commentLine.StartsWith("/// <param"))
                        {
                            var match = Regex.Match(
                                commentLine,
                                @"name=""(\w+)"">(.+?)</param>"
                            );
                            if (match.Success)
                            {
                                pendingParamDesc[match.Groups[1].Value] = match.Groups[2].Value.Trim();
                            }
                        }
                        j++;
                    }

                    // 查找方法定义行（可能跨越多行）
                    if (j < lines.Length)
                    {
                        StringBuilder methodSignature = new StringBuilder();
                        int braceCount = 0;
                        bool foundMethodStart = false;

                        // 从j行开始，直到找到完整的方法定义
                        for (int k = j; k < lines.Length; k++)
                        {
                            string methodLine = lines[k];
                            methodSignature.Append(methodLine);

                            // 检查是否有左括号
                            foreach (char c in methodLine)
                            {
                                if (c == '(') braceCount++;
                                if (c == ')') braceCount--;
                            }

                            // 如果找到了方法开始并且括号匹配完成
                            if (methodLine.Contains("(") || foundMethodStart)
                            {
                                foundMethodStart = true;

                                // 检查括号是否匹配完成，并且遇到了{或;
                                if (braceCount == 0 && (methodLine.Contains("{") || methodLine.Contains(";")))
                                {
                                    // 提取方法定义（到第一个{或;之前）
                                    string fullSignature = methodSignature.ToString();
                                    int endIndex = Math.Min(
                                        fullSignature.IndexOf('{') >= 0 ? fullSignature.IndexOf('{') : int.MaxValue,
                                        fullSignature.IndexOf(';') >= 0 ? fullSignature.IndexOf(';') : int.MaxValue
                                    );

                                    if (endIndex < int.MaxValue)
                                    {
                                        fullSignature = fullSignature.Substring(0, endIndex).Trim();
                                    }

                                    // 匹配方法名和参数
                                    var match = Regex.Match(
                                        fullSignature,
                                        @"\b(public|private|protected|internal|static\s+)?\s*([\w\<\>\.]+\s+)?(\w+)\s*\(([^)]*)\)"
                                    );

                                    if (match.Success)
                                    {
                                        string methodName = match.Groups[3].Value;
                                        string paramList = match.Groups[4].Value.Trim();

                                        if (!result.ToString().Contains("方法:"))
                                            result.AppendLine("\n方法:");

                                        // 构造方法签名
                                        StringBuilder methodLineOutput = new StringBuilder();
                                        methodLineOutput.Append($"  {methodName}(");

                                        List<string> paramOutput = new List<string>();

                                        if (!string.IsNullOrEmpty(paramList))
                                        {
                                            // 处理参数列表，处理逗号分隔但排除泛型中的逗号
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
                                                }
                                            }
                                        }

                                        methodLineOutput.Append(string.Join(", ", paramOutput));
                                        methodLineOutput.Append("): ");
                                        methodLineOutput.Append(description);

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
                                        i = k; // 跳过已处理的行
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result.ToString();
        }

    }
