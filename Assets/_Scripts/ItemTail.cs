using UnityEngine;

[CreateAssetMenu(menuName = "OTT/New Item")]
public class ItemTail : ScriptableObject
{
    [Header("Item")]
    [SerializeField] private string itemName = "ITEM NAME";
    [SerializeField] private Vector3 gripRotation = Vector3.zero;
    [SerializeField] private GameObject meshPrefab = null;

    [Header("Instanced")]
    public GameObject meshObject = null;

    public GameObject MeshPrefab => meshPrefab;
    public string ItemName => itemName;
    public Vector3 GripRotation => gripRotation;

    public static ItemTail CreateInstanceOf(ItemTail item)
    {
        ItemTail itemInstance = ItemTail.CreateInstance<ItemTail>();

        itemInstance.name = item.name + " (Instanced)";
        itemInstance.itemName = item.itemName;
        itemInstance.meshPrefab = item.meshPrefab;
        itemInstance.meshObject = item.meshObject;
        itemInstance.gripRotation = item.gripRotation;
        return itemInstance;
    }
}