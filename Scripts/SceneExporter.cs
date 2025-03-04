using System.IO;
using UnityEngine;

public class SceneExporter : MonoBehaviour
{
    private void Start()
    {
        string filePath = "Assets/SceneExport.txt";
        ExportSceneToFile(filePath);
    }

    private void ExportSceneToFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("文件路径不能为空");
            return;
        }

        string directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // 遍历所有根对象
            foreach (GameObject rootObj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                WriteGameObjectInfo(writer, rootObj, 0);
            }
        }

        Debug.Log($"场景信息已导出到 {filePath}");
    }

    private void WriteGameObjectInfo(StreamWriter writer, GameObject obj, int indentLevel)
    {
        // 写入对象名称
        writer.WriteLine($"{new string(' ', indentLevel * 2)}游戏对象: {obj.name}");
        writer.WriteLine($"{new string(' ', indentLevel * 2)}- 组件:");

        // 写入对象的所有组件
        foreach (Component component in obj.GetComponents<Component>())
        {
            if (component != null)
            {
                writer.WriteLine($"{new string(' ', indentLevel * 2)}  - {component.GetType().Name}");
                WriteComponentProperties(writer, component, indentLevel + 2);
            }
        }

        // 递归遍历子对象
        foreach (Transform child in obj.transform)
        {
            WriteGameObjectInfo(writer, child.gameObject, indentLevel + 1);
        }
    }

    private void WriteComponentProperties(StreamWriter writer, Component component, int indentLevel)
    {
        // 获取组件的所有字段
        System.Reflection.FieldInfo[] fields = component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        foreach (System.Reflection.FieldInfo field in fields)
        {
            if (field.IsPublic || field.IsDefined(typeof(SerializeField), false))
            {
                try
                {
                    object value = field.GetValue(component);
                    WriteFieldOrPropertyValue(writer, field.Name, value, indentLevel);
                }
                catch (System.Exception ex)
                {
                    writer.WriteLine($"{new string(' ', indentLevel * 2)}      - {field.Name}: [Error: {ex.Message}]");
                }
            }
        }

        // 获取组件的所有属性
        System.Reflection.PropertyInfo[] properties = component.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        foreach (System.Reflection.PropertyInfo property in properties)
        {
            if (property.CanRead && property.GetGetMethod() != null && (property.GetGetMethod().IsPublic || property.IsDefined(typeof(SerializeField), false)))
            {
                try
                {
                    object value = property.GetValue(component);
                    WriteFieldOrPropertyValue(writer, property.Name, value, indentLevel);
                }
                catch (System.Exception ex)
                {
                    writer.WriteLine($"{new string(' ', indentLevel * 2)}      - {property.Name}: [Error: {ex.Message}]");
                }
            }
        }
    }

    private void WriteFieldOrPropertyValue(StreamWriter writer, string name, object value, int indentLevel)
    {
        if (value != null)
        {
            if (value is GameObject go)
            {
                writer.WriteLine($"{new string(' ', indentLevel * 2)}      - {name}: {go.name} (Type: {go.GetType().Name})");
            }
            else if (value is Component comp)
            {
                writer.WriteLine($"{new string(' ', indentLevel * 2)}      - {name}: {comp.gameObject.name} (Type: {comp.GetType().Name})");
            }
            else if (value is UnityEngine.Object obj)
            {
                writer.WriteLine($"{new string(' ', indentLevel * 2)}      - {name}: {obj.GetType().Name}");
            }
            else
            {
                writer.WriteLine($"{new string(' ', indentLevel * 2)}      - {name}: {value}");
            }
        }
        else
        {
            writer.WriteLine($"{new string(' ', indentLevel * 2)}      - {name}: null");
        }
    }
}