using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CodeStructureAnalyzer : EditorWindow
{
    private string searchPath = "Assets";
    private string exportPath = "Assets/CodeStructureReport.txt"; // 新增导出路径
    private Dictionary<string, string> codeContents = new Dictionary<string, string>();
    private Vector2 scrollPosition;
    private string selectedFileContent = "";

    [MenuItem("Tools/代码结构分析器")]
    public static void ShowWindow()
    {
        GetWindow<CodeStructureAnalyzer>("代码结构分析器");
    }

    void OnGUI()
    {
        GUILayout.Label("项目代码结构分析", EditorStyles.boldLabel);

        // 路径输入框
        searchPath = EditorGUILayout.TextField("扫描路径:", searchPath);

        // 新增导出路径输入
        exportPath = EditorGUILayout.TextField("导出路径:", exportPath);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("开始扫描"))
        {
            ScanProjectStructure();
        }
        
        // 新增导出按钮
        if (GUILayout.Button("导出为TXT"))
        {
            ExportToTxt();
        }
        GUILayout.EndHorizontal();

        // 显示代码结构树
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        DisplayFileTree(searchPath);
        EditorGUILayout.EndScrollView();

        // 显示选中文件内容
        if (!string.IsNullOrEmpty(selectedFileContent))
        {
            GUILayout.Label("代码内容预览:", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(selectedFileContent, GUILayout.Height(300));
        }
    }

    // 新增TXT导出方法
    private void ExportToTxt()
    {
        try
        {
            StringBuilder report = new StringBuilder();
            
            // 添加标题和时间戳
            report.AppendLine($"代码结构分析报告 - {System.DateTime.Now.ToString("yyyy-MM-dd HH:mm")}");
            report.AppendLine("==================================================\n");

            // 递归生成目录结构
            GenerateTxtStructure(report, searchPath, 0);
            
            // 添加代码内容
            report.AppendLine("\n\n代码文件内容：");
            report.AppendLine("==================================================");
            
            foreach (var entry in codeContents)
            {
                report.AppendLine($"\n// ====== {entry.Key} ======");
                report.AppendLine(entry.Value);
            }

            // 写入文件
            File.WriteAllText(exportPath, report.ToString());
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("导出成功", $"已生成报告到：{exportPath}", "确定");
            EditorUtility.RevealInFinder(exportPath);
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("导出失败", $"错误信息：{e.Message}", "关闭");
        }
    }

    // 生成目录结构（带缩进）
    private void GenerateTxtStructure(StringBuilder sb, string path, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);
        
        // 处理文件夹
        foreach (string dir in Directory.GetDirectories(path))
        {
            string dirName = Path.GetFileName(dir);
            sb.AppendLine($"{indent}📁 {dirName}");
            GenerateTxtStructure(sb, dir, indentLevel + 1);
        }

        // 处理文件
        foreach (string file in Directory.GetFiles(path))
        {
            if (Path.GetExtension(file) == ".cs")
            {
                string fileName = Path.GetFileName(file);
                sb.AppendLine($"{indent}📄 {fileName}");
            }
        }
    }

    // 以下原有方法保持不变
    private void ScanProjectStructure()
    {
        codeContents.Clear();
        TraverseDirectory(searchPath);
        AssetDatabase.Refresh();
    }

    private void TraverseDirectory(string path)
    {
        foreach (string file in Directory.GetFiles(path))
        {
            if (Path.GetExtension(file) == ".cs")
            {
                string content = File.ReadAllText(file);
                codeContents[file] = content;
            }
        }

        foreach (string dir in Directory.GetDirectories(path))
        {
            TraverseDirectory(dir);
        }
    }

    private void DisplayFileTree(string path, int indentLevel = 0)
    {
        string indent = new string(' ', indentLevel * 2);

        foreach (string dir in Directory.GetDirectories(path))
        {
            string dirName = Path.GetFileName(dir);
            if (EditorGUILayout.Foldout(true, $"{indent}📁 {dirName}"))
            {
                DisplayFileTree(dir, indentLevel + 1);
            }
        }

        foreach (string file in Directory.GetFiles(path))
        {
            if (Path.GetExtension(file) == ".cs")
            {
                string fileName = Path.GetFileName(file);
                if (GUILayout.Button($"{indent}📄 {fileName}", EditorStyles.label))
                {
                    selectedFileContent = codeContents[file];
                }
            }
        }
    }
}