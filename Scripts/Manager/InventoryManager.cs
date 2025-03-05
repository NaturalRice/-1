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
            Debug.LogWarning("�㴫�ݵ�type��" + type + "�����ڣ��޷��õ���Ʒ��Ϣ��");
            return null;
        }
    }

    public void AddToBackpack(ItemType type)
    {
        ItemData item = GetItemData(type);
        if (item == null) return;

        // ����������λ���ҵ�����ӵ�λ��
        foreach (SlotData slotData in backpack.slotList)
        {
            if (slotData.item == item && slotData.CanAddItem())
            {
                slotData.Add();
                Debug.Log($"���� {item.name} ����ӵ�����");
                return;
            }
        }

        // ���û���ҵ��ɶѵ��Ĳ�λ��ʹ�ÿղ�λ
        foreach (SlotData slotData in backpack.slotList)
        {
            if (slotData.IsEmpty())
            {
                slotData.AddItem(item);
                Debug.Log($"���� {item.name} ����ӵ��²�λ");
                return;
            }
        }
        Debug.LogWarning("����������");
    }
    
    // ��̬ע����ItemData
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