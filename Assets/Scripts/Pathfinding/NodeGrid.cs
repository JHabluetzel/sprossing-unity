using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeGrid : MonoBehaviour
{
    [SerializeField] private Grid grid;

    public Node[,,] nodes;
    public Vector2Int gridSize;
    private Vector3 bottomLeft;
    private Tilemap[] tilemaps;

    private int layerCount = 3;

    public int MaxSize
    {
        get
        {
            return gridSize.x * gridSize.y * layerCount;
        }
    }

    private void Awake()
    {
        tilemaps = grid.transform.GetComponentsInChildren<Tilemap>();
    }

    public void GenerateGrid(Vector2Int gridSize)
    {
        this.gridSize = gridSize;
        bottomLeft = new Vector3(-gridSize.x / 2f * grid.cellSize.x + grid.cellSize.x, -gridSize.y / 2f * grid.cellSize.y, 0f);

        nodes = new Node[gridSize.x, gridSize.y, layerCount];

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int i = 0; i < tilemaps.Length; i += 3) //bottom to top
                {
                    SeasonalRuleTile tile = tilemaps[i].GetTile<SeasonalRuleTile>(new Vector3Int(x - gridSize.x / 2, y - gridSize.y / 2, 0));
                    if (tile != null)
                    {
                        if (nodes[x, y, i / 3] == null)
                        {
                            nodes[x, y, i / 3] = new Node(bottomLeft + new Vector3(x * grid.cellSize.x, y * grid.cellSize.y, 0), x, y, i / 3);
                        }

                        if (tile.tileType == TileType.Grass)
                        {
                            nodes[x, y, i / 3].movementPenalty = 5;

                            tile = tilemaps[i + 2].GetTile<SeasonalRuleTile>(new Vector3Int(x - gridSize.x / 2, y - gridSize.y / 2, 0));
                            if (tile != null)
                            {
                                nodes[x, y, i / 3].walkID = 0;
                            }
                            else
                            {
                                tile = tilemaps[i + 1].GetTile<SeasonalRuleTile>(new Vector3Int(x - gridSize.x / 2, y - gridSize.y / 2, 0));
                                if (tile != null)
                                {
                                    switch (tile.tileType)
                                    {
                                        case TileType.Cliff:
                                        case TileType.Waterfall:
                                            nodes[x, y, i / 3].walkID = 0;
                                            break;
                                        case TileType.Ramp:
                                            nodes[x, y, i / 3].walkID = 1;
                                            break;
                                        case TileType.Path:
                                            nodes[x, y, i / 3].movementPenalty = 0;
                                            goto default;
                                        default:
                                            nodes[x, y, i / 3].walkID = 2;

                                            break;
                                    }
                                }
                                else
                                {
                                    nodes[x, y, i / 3].walkID = 2;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition, int layer)
    {
        Debug.Log(layer);
        
        Vector3Int cellPosition = tilemaps[0].WorldToCell(worldPosition);
        int checkX = cellPosition.x + gridSize.x / 2;
        int checkY = cellPosition.y + gridSize.y / 2;

        if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
        {
            return nodes[checkX, checkY, layer];
        }

        return null;
    }

    public List<Node> GetNeighbors(Node centerNode)
    {
        List<Node> neighbors = new List<Node>();

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x != 0 || y != 0)
                {
                    int layer = centerNode.layer;
                    int checkX = centerNode.gridX + x;
                    int checkY = centerNode.gridY + y;

                    if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                    {
                        if (centerNode.walkID == 1 && x != 0) //only vertical movement
                        {
                            continue;
                        }

                        if ((x + y) % 2 == 0) //check diagonal
                        {
                            if (nodes[checkX, centerNode.gridY, layer] == null || nodes[centerNode.gridX, checkY, layer] == null)
                            {
                                continue;
                            }
                            else if (nodes[checkX, centerNode.gridY, layer].walkID != 2 || nodes[centerNode.gridX, checkY, layer].walkID != 2)
                            {
                                continue;
                            }
                        }
                        else if (y == 1 && centerNode.walkID == 1) //going up on ramp
                        {
                            if (nodes[checkX, checkY, centerNode.layer].walkID != 1) //leaving ramp
                            {
                                layer++;
                            }
                        }
                        else if (y == -1 && centerNode.walkID != 1) //going down
                        {
                            if (nodes[checkX, checkY, centerNode.layer] == null) //stepping on ramp
                            {
                                layer--;
                            }
                        }

                        if (nodes[checkX, checkY, layer] != null)
                        {
                            if (centerNode.walkID != 2 || x == 0 || nodes[checkX, checkY, layer].walkID != 1) //prevent moving horizontally onto ramp
                            {
                                neighbors.Add(nodes[checkX, checkY, layer]);
                            }
                        }
                    }
                }
            }
        }

        return neighbors;
    }

    public void UpdateNodeInGrid(Vector3 worldPosition, Vector3Int tilePosition)
    {
        for (int j = 0; j < layerCount; j++)
        {
            Node updateNode = GetNodeFromWorldPosition(worldPosition, j);

            if (updateNode == null)
            {
                return;
            }

            nodes[updateNode.gridX, updateNode.gridY, j] = null;

            for (int i = 0; i < tilemaps.Length; i += 3) //bottom to top
            {
                SeasonalRuleTile tile = tilemaps[i].GetTile<SeasonalRuleTile>(tilePosition);
                if (tile != null)
                {
                    if (nodes[updateNode.gridX, updateNode.gridY, j] == null)
                    {
                        nodes[updateNode.gridX, updateNode.gridY, j] = new Node(worldPosition, updateNode.gridX, updateNode.gridY, j);
                    }

                    if (tile.tileType == TileType.Grass)
                    {
                        nodes[updateNode.gridX, updateNode.gridY, j].movementPenalty = 5;

                        tile = tilemaps[i + 2].GetTile<SeasonalRuleTile>(new Vector3Int(updateNode.gridX - gridSize.x / 2, updateNode.gridY - gridSize.y / 2, 0));
                        if (tile != null)
                        {
                            nodes[updateNode.gridX, updateNode.gridY, j].walkID = 0;
                        }
                        else
                        {
                            tile = tilemaps[i + 1].GetTile<SeasonalRuleTile>(new Vector3Int(updateNode.gridX - gridSize.x / 2, updateNode.gridY - gridSize.y / 2, 0));
                            if (tile != null)
                            {
                                switch (tile.tileType)
                                {
                                    case TileType.Cliff:
                                    case TileType.Waterfall:
                                        nodes[updateNode.gridX, updateNode.gridY, j].walkID = 0;
                                        break;
                                    case TileType.Ramp:
                                        nodes[updateNode.gridX, updateNode.gridY, j].walkID = 1;
                                        break;
                                    case TileType.Path:
                                        nodes[updateNode.gridX, updateNode.gridY, j].movementPenalty = 0;
                                        goto default;
                                    default:
                                        nodes[updateNode.gridX, updateNode.gridY, j].walkID = 2;

                                        break;
                                }
                            }
                            else
                            {
                                nodes[updateNode.gridX, updateNode.gridY, j].walkID = 2;
                            }
                        }
                    }
                }
            }
        }
    }

    public bool IsOnBorder(Vector3Int tilePosition)
    {
        if (tilePosition.y == gridSize.y / -2)
        {
            return true;
        }

        if (tilePosition.y == gridSize.y / 2 - 1)
        {
            return true;
        }
        
        if (tilePosition.x == gridSize.x / -2)
        {
            return true;
        }

        if (tilePosition.x == gridSize.x / 2 - 1)
        {
            return true;
        }

        return false;
    }
}