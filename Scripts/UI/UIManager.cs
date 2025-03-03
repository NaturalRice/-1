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

    public GameObject TalkPanelGo0; // 对话界面
    
    [Header("NPC Dialog")]
    public GameObject talkPanel; // 对话面板对象
    public TMP_InputField inputField; // 输入框引用

    private NPCDialog activeNPC; // 当前对话的NPC
    
    // 背包相关字段
    public GameObject inventoryPanel;
    public Transform slotGrid;
    public GameObject slotPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        originalSize = hpMaskImage.rectTransform.rect.width;
        SetHPValue(1);
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
    public void OpenNPCDialog(NPCDialog npc)
    {
        activeNPC = npc;
        talkPanel.SetActive(true);
        inputField.text = ""; // 清空输入框
        PauseGame();
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
    private void PauseGame()
    {
        Time.timeScale = 0f; // 暂停游戏
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    private void ResumeGame()
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
}