using System;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private int inventorySize = 2;
    [SerializeField] private ItemTail[] starterItems = null;
    [SerializeField] private Transform[] inventoryAnchors = null;

    private Action<ItemTail> onEquippedChanged = null;

    private ItemTail[] inventory = null;
    private ItemTail equipped = null;
    public ItemTail Equipped => equipped;
    private void Start()
    {
        SetupInventory();
    }
    private void SetupInventory()
    {
        inventory = new ItemTail[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            inventory[i] = null;
        }
        for (int i = 0; i < starterItems.Length; i++)
        {
            if (i >= inventorySize) return;

            Add(starterItems[i], i);
        }
        UpdateEquipmentVisual();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipToggle(inventory[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipToggle(inventory[1]);
        }
    }
    public void Equip(ItemTail item)
    {
        if(equipped != item)
        {
            if(equipped != null)
            {
                PutItemInSlotVisual(equipped);
            }
            equipped = item;
            Invoke_OnEquippedChanged();
        }
    }
    public void EquipToggle(ItemTail item) // unequip if was equipped
    {
        if (equipped == item) Equip(null);
        else Equip(item);
    }
    public bool CanEquip(ItemTail item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == item) return true;
        }
        return false;
    }
    public bool HasPlace()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null) return true;
        }
        return false;
    }
    public bool TryAdd(ItemTail item)
    {
        for(int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                Add(item, i);
                return true;
            }
        }
        return false;   
    }
    private void Add(ItemTail item, int inventoryIndex)
    {
        ItemTail itemInstance = ItemTail.CreateInstanceOf(item);
        inventory[inventoryIndex] = itemInstance;
        UpdateEquipmentVisual();
    }
    private void UpdateEquipmentVisual()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null || inventory[i].meshObject != null) continue;

            inventory[i].meshObject = Instantiate(inventory[i].MeshPrefab, inventoryAnchors[i]);
            inventory[i].meshObject.layer = InternalSettings.Get.PlayerMask;
            foreach(Transform child in inventory[i].meshObject.transform)
            {
                child.gameObject.layer = InternalSettings.Get.PlayerMask;
            }
        }
    }
    public void PutItemInSlotVisual(ItemTail item)
    {
        for(int i = 0; i < inventoryAnchors.Length; i++)
        {
            if (inventoryAnchors[i].childCount == 0)
            {
                item.meshObject.transform.SetParent(inventoryAnchors[i].transform, false);
                return;
            }
        }
        Debug.LogError("Nowhere to put back " + item.name);
    }

    // Action
    public void AssignOnEquippedChanged(Action<ItemTail> action)
    {
        onEquippedChanged += action;
    }
    private void Invoke_OnEquippedChanged()
    {
        if (onEquippedChanged != null) onEquippedChanged(equipped);
    }
}
