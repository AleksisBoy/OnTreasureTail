using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalSettings : MonoBehaviour
{
    [SerializeField] private LayerMask focusedCameraMask;
    [SerializeField] private LayerMask defaultCameraMask;
    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private LayerMask terrainMask;
    public static InternalSettings Get { get; private set; } = null;
    private void Awake()
    {
        if (Get == null) Get = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    public LayerMask FocusedCameraMask => focusedCameraMask;
    public LayerMask DefaultCameraMask => defaultCameraMask;
    public LayerMask EnvironmentMask => environmentMask;
    public LayerMask TerrainMask => terrainMask;

    public static void CreateDebugSphere(Vector3 position, float scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = position;
        go.transform.localScale *= scale;
    }

    // Terrain
    // ChatGPT
    public static int GetMainTexture(Terrain terrain, Vector3 worldPos)
    {
        // Convert world position to terrain position
        Vector3 terrainPos = worldPos - terrain.transform.position;

        // Normalize the position to a 0-1 range
        float xNormalized = terrainPos.x / terrain.terrainData.size.x;
        float zNormalized = terrainPos.z / terrain.terrainData.size.z;

        // Get the splatmap data
        float[,,] splatmapData = terrain.terrainData.GetAlphamaps(
            (int)(xNormalized * terrain.terrainData.alphamapWidth),
            (int)(zNormalized * terrain.terrainData.alphamapHeight),
            1,
            1
        );

        // Get the texture layer index with the highest weight
        float maxWeight = 0;
        int dominantTextureIndex = 0;

        for (int i = 0; i < splatmapData.GetLength(2); i++)
        {
            if (splatmapData[0, 0, i] > maxWeight)
            {
                maxWeight = splatmapData[0, 0, i];
                dominantTextureIndex = i;
            }
        }

        return dominantTextureIndex;
    }
    // ChatGPT semi
    public static TerrainData CloneTerrainData(TerrainData initialTerrainData)
    {
        TerrainData clonedData = new TerrainData()
        {
            alphamapResolution = initialTerrainData.alphamapResolution,
            baseMapResolution = initialTerrainData.baseMapResolution,
            detailPrototypes = initialTerrainData.detailPrototypes,
            enableHolesTextureCompression = initialTerrainData.enableHolesTextureCompression,
            heightmapResolution = initialTerrainData.heightmapResolution,
            hideFlags = initialTerrainData.hideFlags,
            name = "Cloned_" + initialTerrainData.name,
            size = initialTerrainData.size,
            terrainLayers = initialTerrainData.terrainLayers,
            treeInstances = initialTerrainData.treeInstances,
            treePrototypes = initialTerrainData.treePrototypes,
            wavingGrassAmount = initialTerrainData.wavingGrassAmount,
            wavingGrassSpeed = initialTerrainData.wavingGrassSpeed,
            wavingGrassStrength = initialTerrainData.wavingGrassStrength,
            wavingGrassTint = initialTerrainData.wavingGrassTint
        };
        clonedData.SetHeights(0, 0, initialTerrainData.GetHeights(0, 0, initialTerrainData.heightmapResolution, initialTerrainData.heightmapResolution));
        // Copy splatmap
        for (int i = 0; i < initialTerrainData.alphamapLayers; i++)
        {
            float[,,] alphamaps = initialTerrainData.GetAlphamaps(0, 0, initialTerrainData.alphamapWidth, initialTerrainData.alphamapHeight);
            clonedData.SetAlphamaps(0, 0, alphamaps);
        }

        // Copy detail layers
        for (int i = 0; i < initialTerrainData.detailPrototypes.Length; i++)
        {
            int[,] detailLayer = initialTerrainData.GetDetailLayer(0, 0, initialTerrainData.detailWidth, initialTerrainData.detailHeight, i);
            clonedData.SetDetailLayer(0, 0, i, detailLayer);
        }
        return clonedData;
    }
    // ChatGPT
    public static void LowerTerrainSmooth(Terrain terrain, Vector3 worldPos, float radius, float heightDelta)
    {
        TerrainData terrainData = terrain.terrainData;

        // Convert world position to terrain position
        Vector3 terrainPos = worldPos - terrain.transform.position;

        // Normalize the position to a 0-1 range
        float xNormalized = terrainPos.x / terrainData.size.x;
        float zNormalized = terrainPos.z / terrainData.size.z;

        // Get the corresponding heightmap coordinates
        int heightmapX = Mathf.RoundToInt(xNormalized * terrainData.heightmapResolution);
        int heightmapZ = Mathf.RoundToInt(zNormalized * terrainData.heightmapResolution);

        // Calculate the radius in heightmap space
        int radiusInHeightmap = Mathf.RoundToInt(radius / terrainData.size.x * terrainData.heightmapResolution);

        // Get the heights in the area to modify
        int xStart = Mathf.Max(0, heightmapX - radiusInHeightmap);
        int xEnd = Mathf.Min(terrainData.heightmapResolution, heightmapX + radiusInHeightmap);
        int zStart = Mathf.Max(0, heightmapZ - radiusInHeightmap);
        int zEnd = Mathf.Min(terrainData.heightmapResolution, heightmapZ + radiusInHeightmap);

        float[,] heights = terrainData.GetHeights(xStart, zStart, xEnd - xStart, zEnd - zStart);

        // Modify the heights
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int z = 0; z < heights.GetLength(1); z++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(heightmapX - xStart, heightmapZ - zStart));
                if (distance <= radiusInHeightmap)
                {
                    float modifier = Mathf.Abs(1f - (distance / radiusInHeightmap));
                    heights[x, z] += heightDelta * modifier;
                }
            }
        }

        // Apply the modified heights
        terrainData.SetHeights(xStart, zStart, heights);
    }
}
