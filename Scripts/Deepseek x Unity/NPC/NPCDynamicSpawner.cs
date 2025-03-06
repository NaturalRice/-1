using UnityEngine;
using System.Collections.Generic;
// 在文件头部添加
using UnityEngine.TestTools;       // 单元测试支持
using UnityEngine.UI;             // UI组件
using TMPro;                      // TextMeshPro
using UnityEditor;                // 编辑器相关
using NUnit.Framework;            // 测试框架

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NPCDynamicSpawner : MonoBehaviour
{
    [Header("生成设置")]
    public int maxNPCs = 10;
    public float spawnInterval = 5f;
    public Vector2 spawnArea = new Vector2(20, 20);
    
    public Sprite npcSprite; // 直接引用贴图
    
    public SectConfig[] sectConfigs;

    private List<GameObject> activeNPCs = new List<GameObject>();

    void Start()
    {
        InvokeRepeating(nameof(SpawnNPC), 0f, spawnInterval);
        
        // 测试生成100个NPC
        for(int i=0; i<100; i++) SpawnNPC(); 
    }

    void Update()
    {
#if UNITY_EDITOR
        //Debug.Log($"当前NPC数量：{activeNPCs.Count}");
        //Debug.Log($"内存占用：{UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1024}KB");
#endif
    }

    void SpawnNPC()
    {
        if (activeNPCs.Count >= maxNPCs) return;

        GameObject npc = new GameObject($"NPC_{System.Guid.NewGuid()}");
        npc.tag = "NPC";

        Vector2 spawnPos = GetValidSpawnPosition();
        npc.transform.position = spawnPos;

        // 添加 SpriteRenderer 组件并设置贴图
        SpriteRenderer renderer = npc.AddComponent<SpriteRenderer>();
        renderer.sprite = npcSprite; // 使用直接引用的贴图

        // 设置 sortingLayer 和 sortingOrder
        renderer.sortingLayerName = "NPC"; // 确保在 Unity 中创建了一个名为 "NPC" 的 Sorting Layer
        renderer.sortingOrder = 1; // 设置 sortingOrder，确保 NPC 在地图之上

        Rigidbody2D rb = npc.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        BoxCollider2D collider = npc.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 1.2f);
        collider.isTrigger = true; // 关键设置！

        NPC_AI ai = npc.AddComponent<NPC_AI>();
        ai.Initialize(GetRandomBehavior());
        ai.InitializeBySect((NPC_AI.SectType)Random.Range(0,3)); // 合并初始化

        NPCDialog dialog = npc.AddComponent<NPCDialog>();
        dialog.apiKey = new string[] { "sk-1e2e119a79a14ca1bae80cf4f99a8b0f" }; // 动态注入API Key
        ConfigureDialogComponents(dialog);
        
        npc.AddComponent<NPCLifeTracker>().OnDestroyed += () => activeNPCs.Remove(npc);
        activeNPCs.Add(npc);
        
        Debug.Log($"生成{ai.sectType}门派NPC，攻击力：{ai.attack}，颜色：{ai.sectColor}");
        
        SectConfig config = sectConfigs[Random.Range(0, sectConfigs.Length)];// 随机选择一个门派配置
        ai.InitializeBySect(config.sectType); // 使用配置初始化NPC
        ai.sectColor = config.sectColor; // 设置门派颜色
        
        switch(config.sectType) {
            case NPC_AI.SectType.剑宗: 
                renderer.sprite = Resources.Load<Sprite>("Sprites/NPC/剑宗");
                break;
            case NPC_AI.SectType.丹宗:
                renderer.sprite = Resources.Load<Sprite>("Sprites/NPC/丹宗");
                break;
            case NPC_AI.SectType.符宗:
                renderer.sprite = Resources.Load<Sprite>("Sprites/NPC/符宗");
                break;
        }
        
        // 加载对应门派贴图
        string spritePath = $"Sprites/Sects/{config.sectType}";
        Sprite sectSprite = Resources.Load<Sprite>(spritePath);
    
        // 动态设置SpriteRenderer
        SpriteRenderer Renderer = npc.AddComponent<SpriteRenderer>();
        Renderer.sprite = sectSprite;
    
        // 动态设置颜色（从配置读取）
        Renderer.color = config.sectColor;
    }

    public Vector2 GetValidSpawnPosition()
    {
        Vector2 pos;
        int attempts = 0;
        do
        {
            pos = new Vector2(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                Random.Range(-spawnArea.y / 2, spawnArea.y / 2)
            );
            attempts++;
        } while (Physics2D.OverlapCircle(pos, 1f) != null && attempts < 50);

        return pos;
    }

    NPCBehavior GetRandomBehavior()
    {
        NPCBehavior[] behaviors = Resources.LoadAll<NPCBehavior>("AIBehaviors");
        return behaviors.Length > 0 ? behaviors[Random.Range(0, behaviors.Length)] :
            ScriptableObject.CreateInstance<NPCBehavior>();
    }

    void ConfigureDialogComponents(NPCDialog dialog)
    {
        GameObject bubble = new GameObject("Bubble");
        bubble.transform.SetParent(dialog.transform);

        // 添加 Image 组件到父对象
        Image bubbleImage = bubble.AddComponent<Image>();
        bubbleImage.sprite = dialog.userBubbleSprite; // 根据需要设置默认贴图

        // 创建子对象用于显示文本
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(bubble.transform);
        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
    
        // 配置文本组件
        textComponent.alignment = TextAlignmentOptions.MidlineLeft;
        textComponent.fontSize = 14;

        // 将组件绑定到 NPCDialog
        DiscussionBubble discussionBubble = bubble.AddComponent<DiscussionBubble>();
        discussionBubble.messageText = textComponent;
        discussionBubble.bubbleImage = bubbleImage;
    }

    void OnApplicationQuit()
    {
        foreach (var npc in activeNPCs.ToArray())
        {
            if (npc != null) Destroy(npc);
        }
    }
    
    // 自动生成门派配置的工具类
#if UNITY_EDITOR
public static class SectConfigGenerator {
    public static void GenerateConfig(NPC_AI.SectType type, Color color) {
        var config = ScriptableObject.CreateInstance<SectConfig>();
        config.sectType = type;
        config.sectColor = color;
        UnityEditor.AssetDatabase.CreateAsset(config, $"Assets/Resources/Data/{type}Config.asset");
    }
}
#endif
    
}

[System.Serializable]
public class NPCSaveData
{
    public Vector3 position;
    public string spritePath;
}

// NPC生命周期追踪器
public class NPCLifeTracker : MonoBehaviour
{
    public System.Action OnDestroyed;

    void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}