using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum GridType
    {
        Air,
        Wall
    }

    public GridType[,] map { get; private set; }

    [SerializeField] Transform player;
    [SerializeField] Transform leftEdge;
    [SerializeField] Transform rightEdge;

    [Header("Map Settings")]
    [SerializeField] float size = 1f;
    [SerializeField] int mapWidth = 50;
    [SerializeField] int mapHeight = 50;
    [SerializeField] Vector3 centerOffset;
    [SerializeField] bool startGridFromOrigin;

    [Header("Debug Settings")]
    [SerializeField] bool visualizePlayerGridLocation;
    [SerializeField] bool visualizeMap;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize map array
        map = new GridType[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                map[x, y] = GridType.Air;
            }
        }

        //Gather all walls in the scene
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        //Use the walls position on the map to mark grid spaces with walls
        foreach(GameObject wall in walls)
        {
            Vector2Int index = GetGridLocation(wall.transform.position);
            map[index.x, index.y] = GridType.Wall;
        }

    }

    /// <summary>
    /// Takes in a map grid location and returns the grid type [air, wall, etc.]
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GridType SampleGrid(Vector2Int pos)
    {
        if (pos.x >= mapWidth || pos.y >= mapHeight)
            return GridType.Air;
        return map[pos.x, pos.y];
    }
    /// <summary>
    /// Takes in a location on the map and returns the grid type [air, wall, etc.] at that location
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GridType SampleGrid(Vector3 pos)
    {
        Vector2Int gridLoc = GetGridLocation(pos);
        if (gridLoc.x >= mapWidth || gridLoc.y >= mapHeight)
            return GridType.Air;
        return map[gridLoc.x, gridLoc.y];
    }

    /// <summary>
    /// Takes in a position on the map and returns the integered location on the map grid
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector2Int GetGridLocation(Vector3 pos)
    {
        Vector3 offset = Vector3.zero;
        if (!startGridFromOrigin)
            offset = -new Vector3(mapWidth, 0, mapHeight) / 2f * size;

        Vector3 adjustedPos = pos - offset - transform.position - centerOffset + Vector3.one * size / 2;

        return new Vector2Int(Mathf.Clamp(Mathf.FloorToInt(adjustedPos.x / size), 0, mapWidth - 1), Mathf.Clamp(Mathf.FloorToInt(adjustedPos.z / size), 0, mapHeight - 1));
    }

    /// <summary>
    /// Returns the world position of the given grid position
    /// </summary>
    /// <param name="gridPosition"></param>
    /// <returns></returns>
    public Vector3 GetWorldFromGrid(Vector2Int gridPosition)
    {
        Vector3 offset = Vector3.zero;
        if (!startGridFromOrigin)
            offset = -new Vector3(mapWidth, 0, mapHeight) / 2f * size;

        Vector3 worldPos = new Vector3(gridPosition.x, 0, gridPosition.y) * size + offset + transform.position + centerOffset;

        return worldPos;
    }

    /// <summary>
    /// Return the player position intgered on the map grid
    /// </summary>
    public Vector2Int GetPlayerPosition()
    {
        return GetGridLocation(player.position);
    }

    /// <summary>
    /// Takes in the ghost position on the map and returns position to get to the player the quickest
    /// </summary>
    public Vector2Int GetPlayerPosition(Vector3 ghostPosition)
    {
        Vector2Int playerGridPos = GetGridLocation(player.position);
        Vector2Int ghostGridPos = GetGridLocation(ghostPosition);
        Vector2Int leftEdgeGridPos = GetGridLocation(leftEdge.position);
        Vector2Int rightEdgeGridPos = GetGridLocation(rightEdge.position);

        float distBtwGhostToPlayer = Vector2Int.Distance(playerGridPos, ghostGridPos);

        float distBtwGhostToLeftEdge = Vector2Int.Distance(ghostGridPos, leftEdgeGridPos);
        float distBtwPlayerToLeftEdge = Vector2Int.Distance(playerGridPos, leftEdgeGridPos);

        float distBtwGhostToRightEdge = Vector2Int.Distance(ghostGridPos, rightEdgeGridPos);
        float distBtwPlayerToRightEdge = Vector2Int.Distance(playerGridPos, rightEdgeGridPos);

        if(distBtwGhostToRightEdge < distBtwGhostToLeftEdge)
        {
            if(distBtwGhostToPlayer < distBtwGhostToRightEdge)
            {
                return playerGridPos;
            } 
            else if(distBtwGhostToPlayer < distBtwPlayerToLeftEdge)
            {
                return playerGridPos;
            } 
            else
            {
                return rightEdgeGridPos;
            }
        } 
        else
        {
            if (distBtwGhostToPlayer < distBtwGhostToLeftEdge)
            {
                return playerGridPos;
            }
            else if (distBtwGhostToPlayer < distBtwPlayerToRightEdge)
            {
                return playerGridPos;
            }
            else
            {
                return leftEdgeGridPos;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Debug Function
    private void OnDrawGizmos()
    {
        if(visualizePlayerGridLocation && player)
        {
            Vector3 offset = Vector3.zero;
            if (!startGridFromOrigin)
                offset = -new Vector3(mapWidth, 0, mapHeight) / 2f * size;

            Vector2Int gridLocation = GetGridLocation(player.position);
            print(gridLocation);

            Vector3 worldPos = new Vector3(gridLocation.x, 0, gridLocation.y) * size + offset + transform.position + centerOffset;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(worldPos, Vector3.one * size / 2);
        }

        if (visualizeMap && map != null)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Vector3 offset = Vector3.zero;
                    if (!startGridFromOrigin)
                        offset = -new Vector3(mapWidth, 0, mapHeight) / 2f * size;

                    Vector3 worldPos = new Vector3(x, 0, y) * size + offset + transform.position + centerOffset;

                    if (SampleGrid(new Vector2Int(x, y)) == GridType.Air)
                    {
                        Gizmos.color = Color.white;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawWireCube(worldPos, Vector3.one * size);

                }
            }
        }

    }
}
