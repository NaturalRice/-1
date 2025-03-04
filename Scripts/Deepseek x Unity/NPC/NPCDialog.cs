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
    
    [Header("主角NPC对话设置")]
    [SerializeField] private DiscussionBubble bubblePrefab;
    [SerializeField] public TMP_InputField inputField; 
    [SerializeField] public Transform bubblesParent; 

    // NPC属性
    [Header("NPC人设")]
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
        // 动态获取预制体（保持原有逻辑）
        bubblePrefab = Resources.Load<DiscussionBubble>("Prefabs/Discussion Bubble");
        if (bubblePrefab == null) Debug.LogError("无法加载DiscussionBubble预制体");
        // 进行认证
        Authenticate();

        // 初始化设置
        Initialize();

        // 确保npcName在Start方法中被正确初始化
        //Debug.Log("NPC Name in Start: " + npcName);
        
        // 强制启用 IME 输入
        /*Input.imeCompositionMode = IMECompositionMode.On;
        inputField.onSelect.AddListener((text) => {
            Input.imeCompositionMode = IMECompositionMode.On;
        });*/
    }
    
    void Update()
    {
        if (bubblesParent == null)
        {
            Debug.LogError("气泡父容器在运行时被清空！");
        }
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
    // NPCDialog.cs
    public async void AskButtonCallback()
    {
        // 动态获取当前活动面板的输入框
        TMP_InputField currentInputField = UIManager.Instance.inputField; 
        string playerRequest = currentInputField.text; // 使用当前输入框内容

        CreateBubble(playerRequest, true);
        await ProcessNPCResponse(playerRequest);

        currentInputField.text = ""; // 清空当前输入框
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
    // NPCDialog.cs 新增方法
    public void ShowTrainingMenu()
    {
        CreateBubble($"选择训练方向：\n1.剑道修行\n2.丹术研习\n3.符法修炼", false);
    }
    
    public void OnTrainingSelected(int option)
    {
        NPC_AI ai = GetComponent<NPC_AI>();
        switch(option){
            case 1: 
                ai.attack += 5f;
                CreateBubble($"{ai.sectType}攻击力提升至{ai.attack}!", false);
                break;
            case 2:
                ai.intelligence += 3f;
                break;
        }
        GenerateTexture.Instance.PlayCultivationEffect(transform.position); // 播放修炼特效
    }
    
}