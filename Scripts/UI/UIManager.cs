using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image hpMaskImage;
    public Image mpMaskImage;
    private float originalSize;

    public GameObject TalkPanelGo0; // 对话界面
    
    // 背包相关字段
    public GameObject inventoryPanel;
    public Transform slotGrid;
    public GameObject slotPrefab;

    void Awake()
    {
        Instance = this;
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

    /// <summary>
    /// 关闭对话界面
    /// </summary>
    public void CloseTalkPanel()
    {
        if (TalkPanelGo0 != null)
        {
            TalkPanelGo0.SetActive(false);
            ResumeGame(); // 恢复游戏
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