using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Digging something out
// Walkable areas
public class IslandManager : MonoBehaviour
{
    [SerializeField] private Terrain islandTerrain = null;
    [SerializeField] private float diggingDistanceRestrictionModifier = 4f;
    [SerializeField] private int sandLayerIndex = 1;
    [SerializeField] private InformationProgressBar informationBar = null;

    private List<Vector3> diggingPositions = new List<Vector3>();
    private TerrainData initialTerrainData = null;
    public int SandLayerIndex => sandLayerIndex;
    public Terrain Terrain => islandTerrain;

    private List<InfoSO> info = new List<InfoSO>();
    private List<IInteractable> interactables = new List<IInteractable>();
    public static IslandManager Instance { get; private set; } = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        UIManager.Clear();

        TerrainDataSetup();
    }

    private void TerrainDataSetup()
    {
        initialTerrainData = islandTerrain.terrainData;
        TerrainData clonedData = InternalSettings.CloneTerrainData(initialTerrainData);

        islandTerrain.terrainData = clonedData;
        islandTerrain.GetComponent<TerrainCollider>().terrainData = clonedData;
    }
    public bool DigIslandTerrain(Vector3 worldPos, float diggingRadius, float diggingHeightDelta)
    {
        InternalSettings.LowerTerrainSmooth(islandTerrain, worldPos, diggingRadius, diggingHeightDelta);
        diggingPositions.Add(worldPos);

        return true;
    }
    public bool CanDigAt(Vector3 worldPos, float diggingRadius)
    {
        foreach (Vector3 pos in diggingPositions)
        {
            float distance = Vector3.Distance(worldPos, pos);
            if (distance < diggingRadius * diggingDistanceRestrictionModifier)
            {
                Debug.Log("Too close to digged position");
                return false;
            }
        }
        return true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string debug = "Currently open: ";
            foreach( var panel in UIManager.Current)
            {
                debug += panel.name + "\n";
            }
            Debug.Log(debug);
        }
    }
    public bool AssignInteractable(IInteractable interactable)
    {
        if(interactables.Contains(interactable)) return false;
        interactables.Add(interactable);
        return true;
    }
    public IInteractable GetClosestInteractable(Vector3 position)
    {
        IInteractable closestInteractable = null;
        float closestDistance = 1000f;
        foreach(IInteractable interactable in interactables)
        {
            if (!interactable.InteractionActive()) continue;

            float distance = Vector3.Distance(interactable.GetInteractionPosition(), position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }
    public IInteractable GetClosestInteractable(Vector3 position, out float closestDistance)
    {
        IInteractable closestInteractable = null;
        closestDistance = 1000f;
        foreach(IInteractable interactable in interactables)
        {
            if (!interactable.InteractionActive()) continue;

            float distance = Vector3.Distance(interactable.GetInteractionPosition(), position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }
    public void AddInfo(InfoSO newInfo)
    {
        if (info.Contains(newInfo))
        {
            Debug.LogWarning("Same info added " + newInfo.name);
            return;
        }
        informationBar.AddInfoToBar(newInfo);
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
