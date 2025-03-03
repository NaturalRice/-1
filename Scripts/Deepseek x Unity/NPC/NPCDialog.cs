using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using OpenAI;
using OpenAI.Chat;
using Cysharp.Threading.Tasks;
using TMPro; // 添加在文件头部

public class NPCDialog : MonoBehaviour
{
    // UI元素
    private DiscussionBubble bubblePrefab;
    private TMP_InputField inputField;
    [SerializeField] private Transform bubblesParent;
    
    // 在 NPC 属性后添加以下字段
    [Header("对话气泡设置")]
    [SerializeField] public Sprite userBubbleSprite; // 用户消息气泡贴图

    // 事件
    public static Action onMessageReceived;

    // 认证信息
    [SerializeField] public string[] apiKey;
    private OpenAIClient api;

    // 设置
    [SerializeField]
    private List<Message> chatPrompts = new List<Message>();

    // NPC属性
    [Header("NPC Settings")]
    [SerializeField] public string npcName = "NPC";
    [SerializeField] private string npcRole = "Generic Role";
    [SerializeField] private string npcTask = "Generic Task";
    [SerializeField] private string npcBackground = "Generic Background";
    [SerializeField] private string npcPersonality = "Generic Personality";

    // 在第一次帧更新之前调用
    void Start()
    {
        // 创建初始消息气泡（该行用于测试）
        //CreateBubble($"你好！我叫{npcName}，是{npcRole}", false);
        // 动态获取组件
        inputField = UIManager.Instance.inputField;
        // 添加空值检查
        if (inputField == null) Debug.LogError("输入框未分配！");
        bubblePrefab = Resources.Load<DiscussionBubble>("Prefabs/Discussion Bubble");
        if (bubblePrefab == null) Debug.LogError("无法加载DiscussionBubble预制体");
        // 进行认证
        Authenticate();

        // 初始化设置
        Initialize();

        // 确保npcName在Start方法中被正确初始化
        //Debug.Log("NPC Name in Start: " + npcName);
        
        // 强制启用 IME 输入
        Input.imeCompositionMode = IMECompositionMode.On;
        inputField.onSelect.AddListener((text) => {
            Input.imeCompositionMode = IMECompositionMode.On;
        });
    }

    /// <summary>
    /// 认证OpenAI API密钥。
    /// </summary>
    private void Authenticate()
    {
        if (apiKey == null || apiKey.Length == 0)
        {
            Debug.LogError("API Key 未配置！");
            return;
        }
        api = new OpenAIClient(new OpenAIAuthentication(apiKey[0]));
    }

    /// <summary>
    /// 初始化聊天提示。
    /// </summary>
    private void Initialize()
    {
        Message prompt = new Message(OpenAI.Role.System, $"你是一个名为{npcName}的{npcRole}，你的主要任务是{npcTask}。你的背景是{npcBackground}，性格特点是{npcPersonality}。");
        chatPrompts.Add(prompt);
    }
    
    /// <summary>
    /// 处理用户提问按钮点击事件。
    /// </summary>
    public async void AskButtonCallback()
    {
        string playerRequest = inputField.text;
        CreateBubble(playerRequest, true);
        await ProcessNPCResponse(playerRequest); // 改为await调用

        inputField.text = "";
    }
    
    // NPCDialog.cs 需补充方法实现
    private async UniTask ProcessNPCResponse(string request)
    {
        Message prompt = new Message(OpenAI.Role.User, request);
        chatPrompts.Add(prompt);

        ChatRequest chatRequest = new ChatRequest(
            messages: chatPrompts,
            model: OpenAI.Models.Model.GPT3_5_Turbo,
            temperature: 0.2);

        try
        {
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
            Message chatResult = new Message(OpenAI.Role.Assistant, result.FirstChoice.ToString());
            chatPrompts.Add(chatResult);
            CreateBubble(result.FirstChoice.ToString(), false);
        }
        catch (Exception e)
        {
            Debug.LogError($"对话处理失败：{e.Message}");
            CreateBubble("思考中...请稍后再试", false);
        }
    
        // 移除重复的生成道具逻辑（原AskButtonCallback中的重复代码）
    }
    
    private Dictionary<string, string> commandCache = new Dictionary<string, string>();
    
    /// <summary>
    /// 创建消息气泡。
    /// </summary>
    /// <param name="message">要显示的消息文本。</param>
    /// <param name="isUserMessage">是否为用户消息。</param>
    public void CreateBubble(string message, bool isUserMessage)
    {
        DiscussionBubble discussionBubble = Instantiate(bubblePrefab, bubblesParent);
        discussionBubble.Configure(message, isUserMessage);

        onMessageReceived?.Invoke();
    }
}