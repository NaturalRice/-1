using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private Dictionary<ItemType, ItemData> itemDataDict = new Dictionary<ItemType, ItemData>();

    [HideInInspector]
    public InventoryData backpack;
    [HideInInspector]
    public InventoryData toolbarData;

    private void Awake()
    {
        Instance = this;
        Init();
    }
    
    private void Init()
    {
        ItemData[] itemDataArray = Resources.LoadAll<ItemData>("Data");
        foreach(ItemData data in itemDataArray)
        {
            itemDataDict.Add(data.type, data);
        }

        backpack = Resources.Load<InventoryData>("Data/Backpack");
        toolbarData = Resources.Load<InventoryData>("Data/Toolbar");
    }

    private ItemData GetItemData(ItemType type)
    {
        ItemData data;
        bool isSuccess = itemDataDict.TryGetValue(type, out data);
        if (isSuccess)
        {
            return data;
        }
        else
        {
            Debug.LogWarning("你传递的type：" + type + "不存在，无法得到物品信息。");
            return null;
        }
    }

    public void AddToBackpack(ItemType type)
    {
        ItemData item = GetItemData(type);
        if (item == null) return;

        foreach(SlotData slotData in backpack.slotList)
        {
            if (slotData.item == item && slotData.CanAddItem())
            {
                slotData.Add();return;
            }
        }

        foreach (SlotData slotData in backpack.slotList)
        {
            if (slotData.count == 0)
            {
                slotData.AddItem(item);return;
            }
        }

        Debug.LogWarning("无法放入仓库，你的背包" + backpack + "已满。");
    }


}