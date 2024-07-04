using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum GridType
    {
        Air,
        Wall,
        Barrier
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
    [SerializeField] bool visualizePlayerNextLocation;

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

        if (leftEdge == null)
            leftEdge = GameObject.FindGameObjectWithTag("LeftEdge")?.transform;

        if (rightEdge == null)
            rightEdge = GameObject.FindGameObjectWithTag("RightEdge")?.transform;

        //Gather all walls in the scene
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        //Use the walls position on the map to mark grid spaces with walls
        foreach(GameObject wall in walls)
        {
            Vector2Int index = GetGridLocation(wall.transform.position);
            map[index.x, index.y] = GridType.Wall;
        }

        //Gather all barriers in the scene
        GameObject[] barriers = GameObject.FindGameObjectsWithTag("Barrier");

        //Use the barriers position on the map to mark grid spaces with barriers
        foreach (GameObject barrier in barriers)
        {
            Vector2Int index = GetGridLocation(barrier.transform.position);
            map[index.x, index.y] = GridType.Barrier;
        }
    }

    /// <summary>
    /// Takes in a map grid location and returns the grid type [air, wall, etc.]
    /// </summary>
    public GridType SampleGrid(Vector2Int pos)
    {
        if (pos.x >= mapWidth || pos.y >= mapHeight)
            return GridType.Wall;
        return map[Mathf.Clamp(pos.x, 0, mapWidth - 1), Mathf.Clamp(pos.y, 0, mapHeight - 1)];
    }
    /// <summary>
    /// Takes in a location on the map and returns the grid type [air, wall, etc.] at that location
    /// </summary>
    public GridType SampleGrid(Vector3 pos)
    {
        Vector2Int gridLoc = GetGridLocation(pos);
        if (gridLoc.x >= mapWidth || gridLoc.y >= mapHeight)
            return GridType.Wall;
        return map[Mathf.Clamp(gridLoc.x, 0, mapWidth - 1), Mathf.Clamp(gridLoc.y, 0, mapHeight - 1)];
    }

    /// <summary>
    /// Takes in a position on the map and returns the integered location on the map grid
    /// </summary>
    public Vector2Int GetGridLocation(Vector3 pos)
    {
        Vector3 offset = Vector3.zero;
        if (!startGridFromOrigin)
            offset = -new Vector3(mapWidth, 0, mapHeight) / 2f * size;

        Vector3 adjustedPos = pos - offset - transform.position - centerOffset + Vector3.one * size / 2;

        return new Vector2Int(Mathf.Clamp(Mathf.FloorToInt(adjustedPos.x / size), 0, mapWidth - 1), Mathf.Clamp(Mathf.FloorToInt(adjustedPos.z / size), 0, mapHeight - 1));
    }

    /// <summary>
    /// Takes in a world-space dir and returns the grid space direction [0,1], [0,-1], [1,0], or [-1,0]
    /// </summary>
    public Vector2Int GetGridSpaceDirection(Vector3 dir)
    {
        Vector2Int integeredDir = Vector2Int.zero;
        dir = dir.normalized;

        float dirAngleToForward = Vector3.Dot(dir, Vector3.forward);
        if (dirAngleToForward >= 0.7f)
        {
            integeredDir = Vector2Int.up;
        }

        float dirAngleToBack = Vector3.Dot(dir, Vector3.back);
        if (dirAngleToBack >= 0.7f)
        {
            integeredDir = Vector2Int.down;
        }

        float dirAngleToLeft = Vector3.Dot(dir, Vector3.left);
        if (dirAngleToLeft >= 0.7f)
        {
            integeredDir = Vector2Int.left;
        }

        float dirAngleToRight = Vector3.Dot(dir, Vector3.right);
        if (dirAngleToRight >= 0.7f)
        {
            integeredDir = Vector2Int.right;
        }

        return integeredDir;
    }

    /// <summary>
    /// Takes in a grid position, integered grid-space direction, and priorities and finds the next grid position along that direction taking into account any walls
    /// </summary>
    public Vector2Int GetNextGridPosition(Vector2Int currentGridPos, Vector2Int dir, bool prioritizeUp, bool prioritizeRight)
    {
        if (map == null)
            return currentGridPos + dir;

        Vector2Int nextGridPos = currentGridPos + dir;
        nextGridPos = new Vector2Int(Mathf.Clamp(nextGridPos.x, 0, mapWidth - 1), Mathf.Clamp(nextGridPos.y, 0, mapHeight - 1));

        if (map[nextGridPos.x, nextGridPos.y] != GridType.Air)
        {
            float dirAngleToUp = Vector2.Dot(dir, Vector2.up);
            float dirAngleToLeft = Vector2.Dot(dir, Vector2.left);

            if(dirAngleToUp <= 0.1f && dirAngleToUp >= -0.1f) //given direction is perpendicular to the up and down dir
            {
                if(prioritizeUp && currentGridPos.y < mapHeight - 1 &&  map[currentGridPos.x, currentGridPos.y + 1] == GridType.Air)
                {
                    nextGridPos = currentGridPos + Vector2Int.up;
                } else
                {
                    nextGridPos = currentGridPos + Vector2Int.down;
                }

            } else if (dirAngleToLeft <= 0.1f && dirAngleToLeft >= -0.1f) //given direction is perpendicular to the left and right dir
            {
                if (prioritizeRight && currentGridPos.x < mapWidth - 1 && map[currentGridPos.x + 1, currentGridPos.y] == GridType.Air)
                {
                    nextGridPos = currentGridPos + Vector2Int.right;
                }
                else
                {
                    nextGridPos = currentGridPos + Vector2Int.left;
                }
            }
        }

        return nextGridPos;
    }

    /// <summary>
    /// Takes in a grid position, integered grid-space direction, and priorities and finds the next grid position along that direction taking into account any walls
    /// </summary>
    public Vector2Int GetNextGridPosition(Vector2Int currentGridPos, Vector2Int dir, bool prioritizeUp, bool prioritizeRight, out Vector2Int usedDir)
    {
        usedDir = dir;
        if (map == null)
            return currentGridPos + dir;

        Vector2Int nextGridPos = currentGridPos + dir;
        nextGridPos = new Vector2Int(Mathf.Clamp(nextGridPos.x, 0, mapWidth - 1), Mathf.Clamp(nextGridPos.y, 0, mapHeight - 1));

        if (map[nextGridPos.x, nextGridPos.y] != GridType.Air)
        {
            float dirAngleToUp = Vector2.Dot(dir, Vector2.up);
            float dirAngleToLeft = Vector2.Dot(dir, Vector2.left);

            if (dirAngleToUp <= 0.1f && dirAngleToUp >= -0.1f) //given direction is perpendicular to the up and down dir
            {
                if (prioritizeUp && currentGridPos.y < mapHeight - 1 && map[currentGridPos.x, currentGridPos.y + 1] == GridType.Air)
                {
                    nextGridPos = currentGridPos + Vector2Int.up;
                    usedDir = Vector2Int.up;
                }
                else if(currentGridPos.y > 0 && map[currentGridPos.x, currentGridPos.y - 1] == GridType.Air)
                {
                    nextGridPos = currentGridPos + Vector2Int.down;
                    usedDir = Vector2Int.down;
                }

            }
            else if (dirAngleToLeft <= 0.1f && dirAngleToLeft >= -0.1f) //given direction is perpendicular to the left and right dir
            {
                if (prioritizeRight && currentGridPos.x < mapWidth - 1 && map[currentGridPos.x + 1, currentGridPos.y] == GridType.Air)
                {
                    nextGridPos = currentGridPos + Vector2Int.right;
                    usedDir = Vector2Int.right;
                }
                else if (currentGridPos.x > 0 && map[currentGridPos.x - 1, currentGridPos.y] == GridType.Air)
                {
                    nextGridPos = currentGridPos + Vector2Int.left;
                    usedDir = Vector2Int.left;
                }
            }
        }

        return nextGridPos;
    }

    /// <summary>
    /// Takes in a grid position, world-space direction, and priorities and finds the next grid position along that direction taking into account any walls
    /// </summary>
    public Vector2Int GetNextGridPosition(Vector2Int currentGridPos, Vector3 dir, bool prioritizeUp, bool prioritizeRight)
    {
        Vector2Int integeredDir = GetGridSpaceDirection(dir);

        return GetNextGridPosition(currentGridPos, integeredDir, prioritizeUp, prioritizeRight);
    }

    /// <summary>
    /// Takes in a grid position, integered grid-space direction, number of spaces ahead, and priorities and returns the grid position in the direction 'spacesAhead' of current grid position
    /// </summary>
    public Vector2Int GetGridPositionAhead(Vector2Int currentGridPos, Vector2Int dir, int spacesAhead, bool prioritizeUp, bool prioritizeRight)
    {
        Vector2Int nextGridPos = currentGridPos;
        for (int i = 0; i < spacesAhead; i++)
        {
            nextGridPos = GetNextGridPosition(nextGridPos, dir, prioritizeUp, prioritizeRight);
        }

        return nextGridPos;
    }

    /// <summary>
    /// Takes in a grid position, world-space direction, number of spaces ahead, and priorities and returns the grid position in the direction 'spacesAhead' of current grid position
    /// </summary>
    public Vector2Int GetGridPositionAhead(Vector2Int currentGridPos, Vector3 dir, int spacesAhead, bool prioritizeUp, bool prioritizeRight)
    {
        Vector2Int integeredDir = GetGridSpaceDirection(dir);

        Vector2Int nextGridPos = currentGridPos;
        Vector2Int currentDir = integeredDir;
        for (int i = 0; i < spacesAhead; i++)
        {
            nextGridPos = GetNextGridPosition(nextGridPos, integeredDir, prioritizeUp, prioritizeRight);
        }

        return nextGridPos;
    }

    /// <summary>
    /// (Experimental) Takes in a grid position, any grid-space direction (not just left, right, up and down), and priorities and returns the grid position in that direction from current grid position
    /// </summary>
    public Vector2Int GetGridPositionAhead(Vector2Int currentGridPos, Vector2Int dir, bool prioritizeUp, bool prioritizeRight)
    {
        if(map == null)
            return currentGridPos + dir;

        //Vector2Int clampedDir = new Vector2Int(Mathf.Clamp(dir.x, -1, 1), Mathf.Clamp(dir.y, -1, 1));
        Vector2Int nextGridPos = currentGridPos + dir;
        nextGridPos = new Vector2Int(Mathf.Clamp(nextGridPos.x, 0, mapWidth - 1), Mathf.Clamp(nextGridPos.y, 0, mapHeight - 1));

        if(map[nextGridPos.x, nextGridPos.y] != GridType.Air)
        {
            if(prioritizeUp && nextGridPos.y < mapHeight - 1 && map[nextGridPos.x, nextGridPos.y + 1] == GridType.Air)
            {
                nextGridPos = nextGridPos + Vector2Int.up;
            } 
            else if (nextGridPos.y > 0 && map[nextGridPos.x, nextGridPos.y - 1] == GridType.Air) 
            {
                nextGridPos = nextGridPos + Vector2Int.down;
            } 
            else if (prioritizeRight && nextGridPos.x < mapWidth - 1 && map[nextGridPos.x + 1, nextGridPos.y] == GridType.Air)
            {
                nextGridPos = nextGridPos + Vector2Int.right;
            }
            else if (nextGridPos.x > 0 && map[nextGridPos.x - 1, nextGridPos.y] == GridType.Air)
            {
                nextGridPos = nextGridPos + Vector2Int.left;
            }
        }

        return nextGridPos;
    }

    /// <summary>
    /// Returns the world position of the given grid position
    /// </summary>
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
    public Vector2Int CheckEdgePositions(Vector3 ghostPosition)
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

    /// <summary>
    /// Takes in the ghost position and a target position and returns the grid position to get to the target the quickest
    /// (Used for pinky because target is not player position)
    /// </summary>
    public Vector2Int CheckEdgePositions(Vector3 ghostPosition, Vector2Int targetGridPos)
    {
        Vector2Int ghostGridPos = GetGridLocation(ghostPosition);
        Vector2Int leftEdgeGridPos = GetGridLocation(leftEdge.position);
        Vector2Int rightEdgeGridPos = GetGridLocation(rightEdge.position);

        float distBtwGhostToTarget = Vector2Int.Distance(targetGridPos, ghostGridPos);

        float distBtwGhostToLeftEdge = Vector2Int.Distance(ghostGridPos, leftEdgeGridPos);
        float distBtwTargetToLeftEdge = Vector2Int.Distance(targetGridPos, leftEdgeGridPos);

        float distBtwGhostToRightEdge = Vector2Int.Distance(ghostGridPos, rightEdgeGridPos);
        float distBtwTargetToRightEdge = Vector2Int.Distance(targetGridPos, rightEdgeGridPos);

        if (distBtwGhostToRightEdge < distBtwGhostToLeftEdge)
        {
            if (distBtwGhostToTarget < distBtwGhostToRightEdge)
            {
                return targetGridPos;
            }
            else if (distBtwGhostToTarget < distBtwTargetToLeftEdge)
            {
                return targetGridPos;
            }
            else
            {
                return rightEdgeGridPos;
            }
        }
        else
        {
            if (distBtwGhostToTarget < distBtwGhostToLeftEdge)
            {
                return targetGridPos;
            }
            else if (distBtwGhostToTarget < distBtwTargetToRightEdge)
            {
                return targetGridPos;
            }
            else
            {
                return leftEdgeGridPos;
            }
        }
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

            Vector3 worldPos = new Vector3(gridLocation.x, 0, gridLocation.y) * size + offset + transform.position + centerOffset;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(worldPos, Vector3.one * size / 2);
        }

        if (visualizePlayerNextLocation && player)
        {
            Vector3 offset = Vector3.zero;
            if (!startGridFromOrigin)
                offset = -new Vector3(mapWidth, 0, mapHeight) / 2f * size;

            Vector2Int gridLocation = GetGridLocation(player.position);

            Vector2Int nextGridLocation = GetGridPositionAhead(gridLocation, new Vector2Int(2,2), true, true);

            Vector3 worldPos = new Vector3(nextGridLocation.x, 0, nextGridLocation.y) * size + offset + transform.position + centerOffset;

            Gizmos.color = Color.blue;
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
