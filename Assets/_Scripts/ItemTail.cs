using UnityEngine;

[CreateAssetMenu(menuName = "OTT/New Item")]
public class ItemTail : ScriptableObject
{
    [SerializeField] private string itemName = "ITEM NAME";
    [SerializeField] private GameObject meshPrefab = null;

    public GameObject meshObject = null;

    public GameObject MeshPrefab => meshPrefab;
    public string ItemName => itemName;

    public static ItemTail CreateInstanceOf(ItemTail item)
    {
        ItemTail itemInstance = ItemTail.CreateInstance<ItemTail>();

        itemInstance.itemName = item.itemName;
        itemInstance.meshPrefab = item.meshPrefab;
        itemInstance.meshObject = item.meshObject;
        return itemInstance;
    }
}