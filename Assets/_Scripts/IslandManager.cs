using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: 
// -Walkable areas
// -Check people's belongings like in golden idol. make it like an unknown magic that player would possess
// it source in the beginning is unknown but later with more info u get u will know that an artifact from isla prudera
// froze everyone who was involved in the search for it. 
// -To have other living people on islands with whom u can talk. It would be people from the Warrick's fleet that
// did not get cursed.
// !-Footsteps >>
// -Water splashes
// -Fog of war
public class IslandManager : MonoBehaviour
{
    [SerializeField] private Terrain islandTerrain = null;
    [SerializeField] private int[] sandLayerIndex = new int[1] { 1 };
    [SerializeField] private InformationBar informationBar = null;

    [Header("Digging")]
    [SerializeField] private Vector3 diggedObjectOffset = Vector3.zero;
    [SerializeField] private Transform[] buriedObjects = null;
    [SerializeField] private float diggingDistanceRestrictionModifier = 4f;

    private Vector3 startPosition = Vector3.zero;
    private List<Vector3> diggingPositions = new List<Vector3>();
    private TerrainData initialTerrainData = null;
    public int[] SandLayerIndex => sandLayerIndex;
    public Terrain Terrain => islandTerrain;
    public Vector3 StartPosition => startPosition;

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
        CombatManager.Clear();

        TerrainDataSetup();
    }
    private void Start()
    {
        startPosition = PlayerInteraction.Instance.transform.position; 
        informationBar.BuildGridViewport();

        foreach(Transform buried in buriedObjects)
        {
            buried.position = new Vector3(buried.position.x, 0f, buried.position.z);
        }
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

        DiggingImpactOnTerrain(worldPos, diggingRadius);

        return true;
    }

    private void DiggingImpactOnTerrain(Vector3 worldPos, float diggingRadius)
    {
        float terrainHeight = islandTerrain.SampleHeight(worldPos) + islandTerrain.transform.position.y;
        Vector3 diggedPosition = new Vector3(worldPos.x, terrainHeight, worldPos.z);
        bool diggedBuried = false;
        foreach (Transform buried in buriedObjects)
        {
            if (buried.gameObject.activeSelf) continue;

            float distance = Vector3.Distance(new Vector3(worldPos.x, 0f, worldPos.z), new Vector3(buried.position.x, 0f, buried.position.z));
            if (distance < diggingRadius * diggingDistanceRestrictionModifier)
            {
                // Activate buried object on digging pos
                diggedBuried = true;
                buried.position = diggedPosition + diggedObjectOffset;
                buried.gameObject.SetActive(true);
            }
        }
        // Create object on nothing digged
        if (!diggedBuried)
        {
            GameObject diggedNothing = ObjectPoolingManager.GetObject(InternalSettings.Get.DiggedNothingPrefab, diggedPosition, Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f));
        }
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
                debug += panel.GetName() + "\n";
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
        info.Add(newInfo);
        informationBar.AddInfoToBar(newInfo);
    }
    public void AddInfo(InfoSO newInfo, Vector2 screenPos)
    {
        if (info.Contains(newInfo))
        {
            Debug.LogWarning("Same info added " + newInfo.name);
            return;
        }
        info.Add(newInfo);
        informationBar.AddInfoToBar(newInfo);

        UserInterface.Instance.GainInfoAt(newInfo.name, screenPos);
    }
    public void AddInfo(Vector2 screenPos, params InfoSO[] newInfoArray)
    {
        foreach(InfoSO newInfo in newInfoArray)
        {
            AddInfo(newInfo, screenPos);
        }
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
