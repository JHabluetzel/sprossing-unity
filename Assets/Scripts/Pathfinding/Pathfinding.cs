using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private NodeGrid nodes;
    [SerializeField] private PathRequestManager requestManager;

    public void StartFindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        StartCoroutine(FindPath(startPosition, targetPosition));
    }

    IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Node[] waypoints = new Node[0];
        bool pathWasFound = false;

        Node startNode = nodes.GetNodeFromWorldPosition(startPosition, Mathf.RoundToInt(startPosition.z));
        Node targetNode = nodes.GetNodeFromWorldPosition(targetPosition, Mathf.RoundToInt(targetPosition.z));

        if (startPosition == targetPosition || startNode == null || targetNode == null || targetNode.walkID == 0)
        {
            requestManager.FinishedProcessingPath(waypoints, pathWasFound);
            yield break;
        }

        Heap<Node> openSet = new Heap<Node>(nodes.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode.Equals(targetNode))
            {
                pathWasFound = true;
                break;
            }

            foreach (Node neighbour in nodes.GetNeighbors(currentNode))
            {
                if (neighbour.walkID == 0 || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;

                if (newCost < neighbour.gCost)
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    openSet.UpdateItem(neighbour);
                }
                else if (!openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    openSet.Add(neighbour);
                }
            }
        }

        yield return null;

        if (pathWasFound)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathWasFound = waypoints.Length > 0;
        }

        requestManager.FinishedProcessingPath(waypoints, pathWasFound);
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int deltaX = Math.Abs(nodeA.gridX - nodeB.gridX);
        int deltaY = Math.Abs(nodeA.gridY - nodeB.gridY);
        int deltaZ = Math.Abs(nodeA.layer - nodeB.layer);

        if (deltaX > deltaY)
        {
            return 14 * deltaY + 10 * (deltaX -  deltaY) + 20 * deltaZ;
        }
        else
        {
            return 14 * deltaX + 10 * (deltaY -  deltaX) + 20 * deltaZ;
        }
    }

    private Node[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (!currentNode.Equals(startNode))
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path.ToArray();
    }
}
