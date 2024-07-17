using UnityEngine;
public class GenerateTexture : MonoBehaviour
{
    public Terrain terrain;
    public Vector3 buildingPosition;
    public float radius = 5.0f;

    private float[,,] originalSplatMapData;

    void Start()
    {
        TerrainData terrainData = terrain.terrainData;
        originalSplatMapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
    }

    public void ChangeTerrainTexture(Vector3Int position, float radius, Vector2 size)
    {
        TerrainData terrainData = terrain.terrainData;
        int alphaMapWidth = terrainData.alphamapWidth;
        int alphaMapHeight = terrainData.alphamapHeight;
        int numTextures = terrainData.alphamapLayers;

        // Convert the world position to the terrain local position
        Vector3 terrainLocalPos = (position - terrain.transform.position) + new Vector3(size.x / 2, 0, size.y / 2);

        // Convert the terrain local position to the splat map position
        int splatX = (int)((terrainLocalPos.x / terrainData.size.x) * alphaMapWidth);
        int splatZ = (int)((terrainLocalPos.z / terrainData.size.z) * alphaMapHeight);

        // Calculate the bounds of the affected area
        int startX = Mathf.Max(0, splatX - (int)radius);
        int endX = Mathf.Min(alphaMapWidth, splatX + (int)radius);
        int startZ = Mathf.Max(0, splatZ - (int)radius);
        int endZ = Mathf.Min(alphaMapHeight, splatZ + (int)radius);

        // Get the affected portion of the splat map
        float[,,] splatMapData = terrainData.GetAlphamaps(startX, startZ, endX - startX, endZ - startZ);

        for (int y = 0; y < endZ - startZ; y++)
        {
            for (int x = 0; x < endX - startX; x++)
            {
                int mapX = startX + x;
                int mapZ = startZ + y;


                // Calculate the distance from the center
                float distSquared = (mapX - splatX) * (mapX - splatX) + (mapZ - splatZ) * (mapZ - splatZ);

                // Introduce noise for rough edges
                float noise = Mathf.PerlinNoise(mapX * 0.1f, mapZ * 0.1f) * radius * 0.5f - radius * 0.25f; // Adjust the scale and range for the desired roughness
                float randomNoise = Random.Range(-radius * 0.1f, radius * 0.1f); // Additional random factor
                float adjustedRadiusSquared = (radius + noise + randomNoise) * (radius + noise + randomNoise);

                if (distSquared < adjustedRadiusSquared)
                {
                    // Calculate the falloff based on the distance
                    float falloff = Mathf.Clamp01(1 - Mathf.Sqrt(distSquared) / (radius + noise + randomNoise));

                    for (int i = 0; i < numTextures; i++)
                    {
                        if (i == 1)  // Assuming the new texture is at index 1
                            splatMapData[y, x, i] = 1; // Apply the new texture with falloff
                        else
                            splatMapData[y, x, i] *= (1 - falloff); // Blend with other textures
                    }
                }
            }
        }

        // Set the modified splat map back to the terrain
        terrainData.SetAlphamaps(startX, startZ, splatMapData);
    }
    void OnApplicationQuit()
    {
        TerrainData terrainData = terrain.terrainData;
        terrainData.SetAlphamaps(0, 0, originalSplatMapData);
    }
}