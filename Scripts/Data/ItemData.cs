using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Seed_Carrot,
    Seed_Tomato,
    Hoe,
    CustomAIItem // 新增动态生成道具类型
}
[CreateAssetMenu()]
public class ItemData :ScriptableObject
{
    public ItemType type=ItemType.None;
    public Sprite sprite;
    public GameObject prefab;
    public int maxCount=1;
    public string description; // 添加描述字段
}
