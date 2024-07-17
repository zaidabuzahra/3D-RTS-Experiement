using Unity.VisualScripting;
using UnityEngine;

public class GridGenDeleteMe : MonoBehaviour
{
    [SerializeField]
    private int width, height;
    [SerializeField]
    private GenerateTexture terrainT;

    public static Cell[,] grid;

    private static GenerateTexture terrainST;

    private void Awake()
    {
        terrainST = terrainT;
        grid = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Vector3 position = new Vector3(x, 0, z);
                #pragma warning restore IDE0090
                grid[x, z] = new Cell(false, CellType.ground/*, position*/);
                /*Debug.DrawLine(position, new Vector3(x, 0, z + 1), Color.white, 100f);
                Debug.DrawLine(position, new Vector3(x + 1, 0, z), Color.white, 100f);*/
            }
        }

        /*Debug.DrawLine(new Vector3(0, 0, height), new Vector3(width, 0, height), Color.white, 100f);
        Debug.DrawLine(new Vector3(width, 0, 0), new Vector3(width, 0, height), Color.white, 100f);*/
    }

    public static void PlaceItem(EntityOS entity, Vector3Int mousePos)
    {
        if (CheckFreeSpace(entity.entitySize, mousePos))
        {
            for (int x = 0; x < entity.entitySize.x; x++)
            {
                for (int z = 0; z < entity.entitySize.y; z++)
                {
                    grid[mousePos.x + x, mousePos.z + z].ChangeOccupation(true);
                }
            }
            Instantiate(entity.entityPrefab, mousePos, Quaternion.identity);
            terrainST.ChangeTerrainTexture(mousePos, entity.entitySize.x * 4, entity.entitySize);
        }
    }

    public static bool CheckFreeSpace(Vector2Int size, Vector3Int mousePos)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                if (mousePos.x + x >= grid.GetLength(0) || mousePos.z + z >= grid.GetLength(1)) return false;
                if (grid[mousePos.x + x, mousePos.z + z].Occupied)
                {
                    //Debug.Log($"Occupied at cell: {mousePos.x + x}, {mousePos.z + z}"); 
                    return false;
                }
            }
        }

        return true;
    }
}