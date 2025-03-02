using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int terrainWidth = 512;
    public int terrainLength = 512;
    public int terrainHeight = 50; // Adjusted vertical size multiplier
    public int heightmapResolution = 513;
    public float perlinScale = 20f;

    [Header("Height Adjustment")]
    public float heightMultiplier = 1f;
    public bool invertHeight = false;
    public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Terrain Material")]
    [Tooltip("Assign your custom material here.")]
    public Material terrainMaterial;

    void Start()
    {
        RegenerateTerrain();
    }

    // Public method to regenerate the terrain
    public void RegenerateTerrain()
    {
        // Create a new TerrainData and configure its properties
        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = heightmapResolution,
            size = new Vector3(terrainWidth, terrainHeight, terrainLength)
        };

        // Generate the heightmap using Perlin noise
        float[,] heights = GenerateHeights();
        terrainData.SetHeights(0, 0, heights);

        // Create the Terrain GameObject with the generated TerrainData
        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.transform.position = Vector3.zero;

        // Apply the material to the terrain if one is assigned
        if (terrainMaterial != null)
        {
            Terrain terrain = terrainObject.GetComponent<Terrain>();
            if (terrain != null)
            {
                terrain.materialTemplate = terrainMaterial;
            }
        }
    }

    // Generates a 2D array of height values using Perlin noise
    float[,] GenerateHeights()
    {
        float[,] heights = new float[heightmapResolution, heightmapResolution];
        for (int x = 0; x < heightmapResolution; x++)
        {
            for (int y = 0; y < heightmapResolution; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    // Calculates a height value based on Perlin noise with adjustments
    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / heightmapResolution * perlinScale;
        float yCoord = (float)y / heightmapResolution * perlinScale;
        float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);

        // Invert height if needed (useful if the terrain appears upside down)
        if (invertHeight)
            perlinValue = 1f - perlinValue;

        // Adjust the noise value with the height multiplier and curve
        perlinValue *= heightMultiplier;
        perlinValue = heightCurve.Evaluate(perlinValue);
        
        // Ensure the value remains in the [0, 1] range
        return Mathf.Clamp01(perlinValue);
    }
}