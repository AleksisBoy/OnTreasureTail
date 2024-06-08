using System;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private int inventorySize = 2;
    [SerializeField] private Item[] starterItems = null;
    [SerializeField] private Transform[] inventoryAnchors = null;

    private Action<Item> onEquippedChanged = null;

    private Item[] inventory = null;
    private Item equipped = null;
    public Item Equipped => equipped;
    private void Awake()
    {
        SetupInventory();
    }
    private void SetupInventory()
    {
        inventory = new Item[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            inventory[i] = null;
        }
        for (int i = 0; i < starterItems.Length; i++)
        {
            if (i >= inventorySize) return;

            inventory[i] = starterItems[i];
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
    public void Equip(Item item)
    {
        if(equipped != item)
        {
            if(item == null && equipped != null)
            {
                PutItemInSlotVisual(equipped);
            }
            equipped = item;
            Invoke_OnEquippedChanged();
        }
    }
    public void EquipToggle(Item item) // unequip if was equipped
    {
        if (equipped == item) Equip(null);
        else Equip(item);
    }
    public bool CanEquip(Item item)
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
    public bool TryAdd(Item item)
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
    private void Add(Item item, int inventoryIndex)
    {
        Item itemInstance = Item.CreateInstanceOf(item);
        inventory[inventoryIndex] = itemInstance;
        UpdateEquipmentVisual();
    }
    private void UpdateEquipmentVisual()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].meshObject != null) continue;

            inventory[i].meshObject = Instantiate(inventory[i].MeshPrefab, inventoryAnchors[i]);
        }
    }
    public void PutItemInSlotVisual(Item item)
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
    public void AssignOnEquippedChanged(Action<Item> action)
    {
        onEquippedChanged += action;
    }
    private void Invoke_OnEquippedChanged()
    {
        if (onEquippedChanged != null) onEquippedChanged(equipped);
    }
    [CreateAssetMenu(menuName = "OTT/New Item")]
    public class Item : ScriptableObject
    {
        [SerializeField] private string itemName = "ITEM NAME";
        [SerializeField] private GameObject meshPrefab = null;

        public GameObject meshObject = null;

        public GameObject MeshPrefab => meshPrefab;
        public string ItemName => itemName;

        public static Item CreateInstanceOf(Item item)
        {
            Item itemInstance = Item.CreateInstance<Item>();

            itemInstance.itemName = item.itemName;
            itemInstance.meshPrefab = item.meshPrefab;
            itemInstance.meshObject = item.meshObject;
            return itemInstance;
        }
    }
}
