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

        // 遍历背包槽位，找到可添加的位置
        foreach (SlotData slotData in backpack.slotList)
        {
            if (slotData.item == item && slotData.CanAddItem())
            {
                slotData.Add();
                Debug.Log($"道具 {item.name} 已添加到背包");
                return;
            }
        }

        // 如果没有找到可堆叠的槽位，使用空槽位
        foreach (SlotData slotData in backpack.slotList)
        {
            if (slotData.IsEmpty())
            {
                slotData.AddItem(item);
                Debug.Log($"道具 {item.name} 已添加到新槽位");
                return;
            }
        }
        Debug.LogWarning("背包已满！");
    }
    
    // 动态注册新ItemData
    public void RegisterCustomItem(ItemData itemData)
    {
        if (!itemDataDict.ContainsKey(itemData.type))
        {
            itemDataDict.Add(itemData.type, itemData);
        }
    }
    
    public bool HasItem(ItemType type) {
        return backpack.slotList.Exists(slot => slot.item?.type == type);
    }

    public void UseItem(ItemType type, int count) {
        SlotData slot = backpack.slotList.Find(s => s.item?.type == type);
        if (slot != null) slot.Reduce(count);
    }
}