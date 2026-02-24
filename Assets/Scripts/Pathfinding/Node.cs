using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Vector3 worldPosition;
    public int gridX, gridY;
    public int walkID; //2 = walkable, 1 = ramp, 0 = unwalkable
    public int movementPenalty;
    public Node parent;
    private int heapIndex;
    public int layer { get; private set; }

    public int gCost;
    public int hCost;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Node(Vector3 worldPosition, int gridX, int gridY, int layer)
    {
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        this.layer = layer;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }

    public int GetLevel(int checkLayer, Vector3Int direction)
    {
        int testLayer = checkLayer - 7;
        testLayer = testLayer / 3;

        if (walkID == 0)
        {
            return 0;
        }
        else if (direction.x == 0 && (checkLayer - 1) % 3 != 0) //on ramp
        {
            if (walkID == 1)
            {
                return checkLayer + direction.y;
            }
            else
            {
                return layer * 3 + 7;
            }
        }
        else if (direction.x == 0 && walkID == 1) //go on ramp
        {
            return checkLayer + direction.y;
        }
        else if (testLayer == layer && walkID != 1)
        {
            return checkLayer;
        }

        return 0;
    }
}
