using UnityEngine;

public class AIGameManager : MonoBehaviour
{
    // 动态生成道具
    public void SpawnItem(string itemName, Vector2 position)
    {
        GameObject prefab = Resources.Load<GameObject>($"Items/{itemName}");
        Instantiate(prefab, position, Quaternion.identity);
        Debug.Log($"AI生成道具：{itemName}");
    }
}