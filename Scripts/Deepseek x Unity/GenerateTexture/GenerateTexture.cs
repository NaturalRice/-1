using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;

public class GenerateTexture : MonoBehaviour
{
    public static GenerateTexture Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 认证信息
    [SerializeField] private string[] apiKey;
    private OpenAIClient api;
    
    [SerializeField] private NPCDialog npcDialog;

    private void Start()
    {
        api = new OpenAIClient(new OpenAIAuthentication(apiKey[0]));
    }
    
    public async Task<ItemData> GenerateItemData(string playerRequest)
    {
        string prompt = $@"玩家需要以下道具：{playerRequest}。请生成JSON数据，包含字段：name, type, description, damage, maxCount";
        
        var messages = new List<Message>();
        messages.Add(new Message(Role.User, prompt));

        var request = new ChatRequest(
            messages: messages,
            model: "gpt-3.5-turbo",
            responseFormat: ChatResponseFormat.Json
        );

        var response = await api.ChatEndpoint.GetCompletionAsync(request);
        string json = response.FirstChoice.Message.Content.ToString();
        Debug.Log($"原始JSON数据：{json}");

        ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
        JsonUtility.FromJsonOverwrite(json, itemData);
        // 添加日志验证
        Debug.Log($"生成道具：{itemData.name}, 类型：{itemData.type}, 描述：{itemData.description}");
        return itemData;
    }

    public async UniTask<Sprite> GenerateItemTexture(string itemName, string itemDescription)
    {
        try
        {
            // 1. 生成提示词（要求透明背景）
            string prompt = $"{itemName}, {itemDescription}, pixel art style, isolated on transparent background, no surroundings, clean edges";
            string encodedPrompt = Uri.EscapeDataString(prompt);
        
            // 2. 生成唯一种子
            int timeSeed = DateTime.Now.Millisecond;
            int inputHash = itemName.GetHashCode();
            int seed = (timeSeed + inputHash) % 10000;
        
            // 3. 构建URL（指定PNG格式）
            string imageUrl = $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=1024&height=1024&seed={seed}&model=flux&nologo=true&format=png";
        
            // 4. 下载贴图
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                await webRequest.SendWebRequest().ToUniTask();
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                
                    // 5. 后期去背景（可选）
                    texture = RemoveBackground(texture, Color.clear); // 没有背景色
                    
                    // 6. 将处理后的Texture2D转为Sprite
                    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                }
                else
                {
                    Debug.LogError($"贴图下载失败：{webRequest.error}");
                    return null;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"生成贴图失败：{e.Message}");
            return null;
        }
    }

    public async UniTaskVoid OnPlayerRequestSubmitted(string playerRequest)
    {
        // 1. 生成道具数据
        ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
        itemData.name = playerRequest; 
        itemData.type = ItemType.CustomAIItem;
        itemData.description = "AI生成的神器"; 

        // 2. 生成贴图（关键修正：动态传递玩家输入作为描述）
        Sprite itemSprite = await GenerateItemTexture(itemData.name, itemData.description);
        if (itemSprite == null)
        {
            npcDialog.CreateBubble("生成道具失败！", false);
            return;
        }
        itemData.sprite = itemSprite;

        // 3. 注册新道具到背包系统
        InventoryManager.Instance.RegisterCustomItem(itemData);

        // 4. 添加到背包
        InventoryManager.Instance.AddToBackpack(itemData.type);
        npcDialog.CreateBubble($"已生成道具：{itemData.name}", false);
    }
    
    public static Texture2D RemoveBackground(Texture2D texture, Color backgroundColor, float threshold = 0.1f)
    {
        Color[] pixels = texture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (ColorDistance(pixels[i], backgroundColor) < threshold)
            {
                pixels[i] = Color.clear;
            }
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture; // 直接返回处理后的Texture2D
    }

    private static float ColorDistance(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b);
    }
    
    [Header("修真特效")]
    public GameObject cultivationEffectPrefab; // 新增字段

    public void PlayCultivationEffect(Vector3 position)
    {
        if(cultivationEffectPrefab != null){
            GameObject effect = Instantiate(cultivationEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
    
    public void PlayCultivationEffect(Vector3 position, NPC_AI.SectType sect) {
        GameObject effect = Instantiate(GetSectEffect(sect), position, Quaternion.identity);
        Destroy(effect, 2f);
    }
    
    [Header("门派特效")]
    public GameObject swordEffect; // 剑宗特效
    public GameObject alchemyEffect; // 丹宗特效
    public GameObject talismanEffect; // 符宗特效
    public GameObject defaultEffect; // 默认特效

    private GameObject GetSectEffect(NPC_AI.SectType sect) {
        return sect switch {
            NPC_AI.SectType.剑宗 => swordEffect,
            NPC_AI.SectType.丹宗 => alchemyEffect,
            NPC_AI.SectType.符宗 => talismanEffect,
            _ => defaultEffect
        };
    }
}