using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class CodeStructureAnalyzer : EditorWindow
{
    private string exportPath = "Assets/FileStructureReport.txt"; // 默认导出路径

    [MenuItem("Tools/文件结构分析器")]
    public static void ShowWindow()
    {
        GetWindow<CodeStructureAnalyzer>("文件结构分析器");
    }

    void OnGUI()
    {
        GUILayout.Label("项目文件结构分析", EditorStyles.boldLabel);

        // 导出路径输入
        exportPath = EditorGUILayout.TextField("导出路径:", exportPath);

        if (GUILayout.Button("生成文件结构报告"))
        {
            ExportFileStructure();
        }
    }

    // 导出文件结构
    private void ExportFileStructure()
    {
        try
        {
            StringBuilder report = new StringBuilder();

            // 添加标题和时间戳
            report.AppendLine($"文件结构分析报告 - {System.DateTime.Now.ToString("yyyy-MM-dd HH:mm")}");
            report.AppendLine("==================================================\n");

            // 递归生成目录结构
            GenerateFileStructure(report, "Assets", 0);

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

    // 生成文件结构（带缩进）
    private void GenerateFileStructure(StringBuilder sb, string path, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);

        // 处理文件夹
        foreach (string dir in Directory.GetDirectories(path))
        {
            string dirName = Path.GetFileName(dir);
            sb.AppendLine($"{indent}📁 {dirName}");
            GenerateFileStructure(sb, dir, indentLevel + 1);
        }

        // 处理文件
        foreach (string file in Directory.GetFiles(path))
        {
            string fileName = Path.GetFileName(file);
            sb.AppendLine($"{indent}📄 {fileName}");
        }
    }
}
