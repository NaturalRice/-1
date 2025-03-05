using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 添加在文件头部

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image hpMaskImage;
    public Image mpMaskImage;
    private float originalSize;
    // 对话界面
    public GameObject TalkPanelGo0;//玩家及其它NPC 
    public GameObject TalkPanelGo1;//剑灵AI 
    public GameObject TalkPanelGo2;//丹灵AI 
    public GameObject TalkPanelGo3;//符灵AI 
    
    [Header("NPC Dialog")]
    public GameObject talkPanel; // 对话面板对象
    public TMP_InputField inputField; // 输入框引用

    private NPCDialog activeNPC; // 当前对话的NPC
    
    [Header("训练系统")]
    public GameObject trainingButtonPrefab; // 新增字段：按钮预制体
    public Transform trainingPanel;         // 新增字段：按钮容器

    void Awake()
    {
        if (Instance == null) Instance = this;
        originalSize = hpMaskImage.rectTransform.rect.width;
        SetHPValue(1);
    }
    
    public void ShowDialog(string name)
    {
        PauseGame(); // 暂停游戏

        // 根据NPC名称打开对应的对话面板
        switch (name)
        {
            case "剑灵AI":
                TalkPanelGo1.SetActive(true);
                break;
            case "丹灵AI":
                TalkPanelGo2.SetActive(true);
                break;
            case "符灵AI":
                TalkPanelGo3.SetActive(true);
                break;
            default:
                TalkPanelGo0.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 打开对话界面
    /// </summary>
    public void OpenTalkPanel()
    {
        if (TalkPanelGo0 != null)
        {
            TalkPanelGo0.SetActive(true);
            PauseGame(); // 暂停游戏
        }
    }
    
    // 打开NPC专属对话面板
    // UIManager.cs 的 OpenNPCDialog 方法
    public void OpenNPCDialog(NPCDialog npc)
    {
        activeNPC = npc;
        ShowDialog(activeNPC.npcName);

        // 动态绑定输入框和父容器
        switch (activeNPC.npcName)
        {
            case "剑灵AI":
                npc.inputField = TalkPanelGo1.GetComponentInChildren<TMP_InputField>(true); // 包含未激活的子对象
                npc.bubblesParent = TalkPanelGo1.transform.Find("BubblesParent");
                break;
            case "丹灵AI":
                npc.inputField = TalkPanelGo2.GetComponentInChildren<TMP_InputField>(true);
                npc.bubblesParent = TalkPanelGo2.transform.Find("BubblesParent");
                break;
            case "符灵AI":
                npc.inputField = TalkPanelGo3.GetComponentInChildren<TMP_InputField>(true);
                npc.bubblesParent = TalkPanelGo3.transform.Find("BubblesParent");
                break;
        }

        // 添加空值检查
        if (npc.inputField == null) Debug.LogError("输入框绑定失败！");
        if (npc.bubblesParent == null) Debug.LogError("气泡父容器绑定失败！");

        PauseGame();
        
        // 在原有代码后添加
        if(npc.GetComponent<NPC_AI>().sectType == NPC_AI.SectType.剑宗){
            // 新增训练按钮方法
            ShowTrainingButtons("剑道", "御剑", "心法"); 
        }
    }

    /// <summary>
    /// 关闭对话界面
    /// </summary>
    public void CloseTalkPanel()
    {
        activeNPC = null;
        talkPanel.SetActive(false);
        ResumeGame();
    }
    
    // 提问按钮回调（绑定到UI按钮）
    public void OnAskButtonClicked()
    {
        if (activeNPC != null && !string.IsNullOrEmpty(inputField.text))
        {
            activeNPC.AskButtonCallback();
        }
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f; // 暂停游戏
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f; // 恢复游戏
    }

    /// <summary>
    /// 血条UI填充显示
    /// </summary>
    public void SetHPValue(float fillPercent)
    {
        hpMaskImage.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, fillPercent * originalSize);
    }

    /// <summary>
    /// 蓝条UI填充显示
    /// </summary>
    public void SetMPValue(float fillPercent)
    {
        mpMaskImage.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, fillPercent * originalSize);
    }
    
    public void ShowTrainingButtons(params string[] buttons) 
    {
        // 清空现有按钮
        foreach(Transform child in trainingPanel){
            Destroy(child.gameObject);
        }

        // 生成新按钮
        foreach (var btnText in buttons) 
        {
            GameObject btn = Instantiate(trainingButtonPrefab, trainingPanel);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = btnText;
            
            // 添加按钮点击监听
            Button buttonComp = btn.GetComponent<Button>();
            int index = System.Array.IndexOf(buttons, btnText) + 1;
            buttonComp.onClick.AddListener(() => {
                activeNPC.OnTrainingSelected(index);
            });
        }
    }

    
    // 临时添加测试按钮（在UIManager.cs中）
    void OnGUI() {
        if(GUILayout.Button("模拟训练")) {
            activeNPC.OnTrainingSelected(1); // 触发剑道训练
        }
    }
    
    [Header("UI预制体")]
    public GameObject attributePopupPrefab; // 属性变化提示

    public void ShowAttributeChange(string attribute, float value) {
        if(attributePopupPrefab != null) {
            GameObject popup = Instantiate(attributePopupPrefab, transform);
            popup.GetComponent<TextMeshProUGUI>().text = $"{attribute}+{value}";
            Destroy(popup, 1.5f);
        }
    }
}