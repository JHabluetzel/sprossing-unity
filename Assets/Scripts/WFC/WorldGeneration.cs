using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private WFCTile[] allTiles;

    public Cell[,] grid;
    private Vector2Int gridSize;

    public void GenerateGrid(Vector2Int gridSizeInChunks)
    {
        gridSize = new Vector2Int(gridSizeInChunks.x, gridSizeInChunks.y);
        grid = new Cell[gridSize.x, gridSize.y];

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                grid[x, y] = new Cell(allTiles);
                grid[x, y].UpdateCell(GetNeighbours(x, y));
            }
        }

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                grid[x, y].UpdateCellWithTop(GetTopNeighbours(x, y));
                grid[x, y].UpdateCellWithRight(GetRightNeighbours(x, y));
                grid[x, y].UpdateCellWithBottom(GetBottomNeighbours(x, y));
                grid[x, y].UpdateCellWithLeft(GetLeftNeighbours(x, y));

                grid[x, y].CollapseCell(allTiles[0]);
            }
        }

        StringBuilder temp = new StringBuilder();

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if (grid[x, y] != null)
                {
                    temp.Append(grid[x, y].options[0].name);
                }
                else
                {
                    temp.Append("-");
                }
            }
            temp.Append("\n");
        }

        Debug.Log(temp);
    }

    private bool[] GetNeighbours(int gridX, int gridY)
    {
        bool[] neighbours = new bool[4]; //top, right, bottom, left

        neighbours[0] = (gridY + 1) < gridSize.y;
        neighbours[1] = (gridX + 1) < gridSize.x;
        neighbours[2] = (gridY - 1) >= 0;
        neighbours[3] = (gridX - 1) >= 0;

        return neighbours;
    }

    private WFCTile[] GetTopNeighbours(int gridX, int gridY)
    {
        if ((gridY + 1) < gridSize.y)
        {
            return grid[gridX,gridY + 1].options.ToArray();
        }
        else
        {
            return new WFCTile[0];
        }
    }

    private WFCTile[] GetRightNeighbours(int gridX, int gridY)
    {
        if ((gridX + 1) < gridSize.x)
        {
            return grid[gridX + 1,gridY].options.ToArray();
        }
        else
        {
            return new WFCTile[0];
        }
    }

    private WFCTile[] GetBottomNeighbours(int gridX, int gridY)
    {
        if ((gridY - 1) >= 0)
        {
            return grid[gridX,gridY - 1].options.ToArray();
        }
        else
        {
            return new WFCTile[0];
        }
    }

    private WFCTile[] GetLeftNeighbours(int gridX, int gridY)
    {
        if ((gridX - 1) >= 0)
        {
            return grid[gridX - 1,gridY].options.ToArray();
        }
        else
        {
            return new WFCTile[0];
        }
    }
}

public class Cell
{
    public List<WFCTile> options;
    public bool IsCollapsed
    {
        get { return options.Count == 1; }
    }

    public Cell(WFCTile[] options)
    {
        this.options = options.ToList();
    }

    public void UpdateCell(bool[] neighbours)
    {
        int index = 0;
        while (index < options.Count)
        {
            if (options[index].topOptions.Length > 0 && !neighbours[0])
            {
                options.RemoveAt(index);
                continue;
            }
            else if (options[index].topOptions.Length == 0 && neighbours[0])
            {
                options.RemoveAt(index);
                continue;
            }

            if (options[index].rightOptions.Length > 0 && !neighbours[1])
            {
                options.RemoveAt(index);
                continue;
            }
            else if (options[index].rightOptions.Length == 0 && neighbours[1])
            {
                options.RemoveAt(index);
                continue;
            }

            if (options[index].bottomOptions.Length > 0 && !neighbours[2])
            {
                options.RemoveAt(index);
                continue;
            }
            else if (options[index].bottomOptions.Length == 0 && neighbours[2])
            {
                options.RemoveAt(index);
                continue;
            }

            if (options[index].leftOptions.Length > 0 && !neighbours[3])
            {
                options.RemoveAt(index);
                continue;
            }
            else if (options[index].leftOptions.Length == 0 && neighbours[3])
            {
                options.RemoveAt(index);
                continue;
            }

            index++;
        }
    }

    public void UpdateCellWithTop(WFCTile[] tiles)
    {
        if (tiles.Length == 0)
        {
            return;
        }

        int index = 0;
        while (index < options.Count)
        {
            if (options[index].topOptions.Length > 0)
            {
                bool wasMatch = false;
                foreach (WFCTile tile in tiles)
                {
                    if (options[index].topOptions.Contains(tile))
                    {
                        wasMatch = true;
                        break; //leaves inner loop
                    }
                }

                if (!wasMatch)
                {
                    options.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            else
            {
                index++;
            }
        }
    }

    public void UpdateCellWithRight(WFCTile[] tiles)
    {
        if (tiles.Length == 0)
        {
            return;
        }

        int index = 0;
        while (index < options.Count)
        {
            if (options[index].rightOptions.Length > 0)
            {
                bool wasMatch = false;
                foreach (WFCTile tile in tiles)
                {
                    if (options[index].rightOptions.Contains(tile))
                    {
                        wasMatch = true;
                        break; //leaves inner loop
                    }
                }

                if (!wasMatch)
                {
                    options.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            else
            {
                index++;
            }
        }
    }

    public void UpdateCellWithBottom(WFCTile[] tiles)
    {
        if (tiles.Length == 0)
        {
            return;
        }

        int index = 0;
        while (index < options.Count)
        {
            if (options[index].bottomOptions.Length > 0)
            {
                bool wasMatch = false;
                foreach (WFCTile tile in tiles)
                {
                    if (options[index].bottomOptions.Contains(tile))
                    {
                        wasMatch = true;
                        break; //leaves inner loop
                    }
                }

                if (!wasMatch)
                {
                    options.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            else
            {
                index++;
            }
        }
    }

    public void UpdateCellWithLeft(WFCTile[] tiles)
    {
        if (tiles.Length == 0)
        {
            return;
        }

        int index = 0;
        while (index < options.Count)
        {
            if (options[index].leftOptions.Length > 0)
            {
                bool wasMatch = false;
                foreach (WFCTile tile in tiles)
                {
                    if (options[index].leftOptions.Contains(tile))
                    {
                        wasMatch = true;
                        break; //leaves inner loop
                    }
                }

                if (!wasMatch)
                {
                    options.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            else
            {
                index++;
            }
        }
    }

    public void CollapseCell(WFCTile temp)
    {
        if (options.Count > 0)
        {
            List<WFCTile> collapsed = new List<WFCTile>{options[UnityEngine.Random.Range(0, options.Count)]};
            options = collapsed;
        }
        else
        {
            options = new List<WFCTile>() {temp};
        }
    }
}