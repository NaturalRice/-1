using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarSlotUI : SlotUI
{
    public Sprite slotLight;
    public Sprite slotDark;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Highlight()
    {
        image.sprite = slotDark;
    }
    public void UnHighlight()
    {
        image.sprite = slotLight;
    }
}
