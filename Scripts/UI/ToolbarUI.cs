using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarUI : MonoBehaviour
{
    public List<ToolbarSlotUI> slotuiList;

    private ToolbarSlotUI selectedSlotUI;//data type

    // Start is called before the first frame update
    void Start()
    {
        InitUI();
    }

    private void Update()
    {
        ToolbarSelectControl();

    }
    public ToolbarSlotUI GetSelectedSlotUI()
    {
        return selectedSlotUI;
    }

    void InitUI()
    {
        slotuiList = new List<ToolbarSlotUI>(new ToolbarSlotUI[9]);
        ToolbarSlotUI[] slotuiArray = transform.GetComponentsInChildren<ToolbarSlotUI>();

        foreach (ToolbarSlotUI slotUI in slotuiArray)
        {
            slotuiList[slotUI.index] = slotUI;
        }

        UpdateUI();
    }
    public void UpdateUI()
    {
        List<SlotData> slotdataList = InventoryManager.Instance.toolbarData.slotList;

        for (int i = 0; i < slotdataList.Count; i++)
        {
            slotuiList[i].SetData(slotdataList[i]);
        }
    }
    void ToolbarSelectControl()
    {
        for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha9; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
            {
                if (selectedSlotUI != null)
                {
                    selectedSlotUI.UnHighlight();
                }
                int index = i - (int)KeyCode.Alpha1;
                selectedSlotUI = slotuiList[index];
                selectedSlotUI.Highlight();
            }
        }
    }
}
