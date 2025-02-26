using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackUI : MonoBehaviour
{
    private GameObject parentUI;

    public List<SlotUI> slotuiList;

    private void Awake()
    {
        parentUI = transform.Find("ParentUI").gameObject;
    }

    private void Start()
    {
        InitUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleUI();
        }
    }

    void InitUI()
    {
        slotuiList = new List<SlotUI>(new SlotUI[24]);  
        SlotUI[] slotuiArray = transform.GetComponentsInChildren<SlotUI>();

        foreach(SlotUI slotUI in slotuiArray)
        {
            slotuiList[slotUI.index] = slotUI;
        }

        UpdateUI();
    }
    public void UpdateUI()
    {
        List<SlotData> slotdataList = InventoryManager.Instance.backpack.slotList;
        
        for(int i = 0; i < slotdataList.Count; i++)
        {
            slotuiList[i].SetData(slotdataList[i]);
        }
    }

    private void ToggleUI()
    {
        parentUI.SetActive(!parentUI.activeSelf);
    }

    public void OnCloseClick()
    {
        ToggleUI();
    }
}
