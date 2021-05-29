using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    [SerializeField] private BlockModel currentBlock;
    [SerializeField] private Vector2 nodeID;
    [SerializeField] private List<Node> neighbourNodes;
    public Vector2 NodeID 
    {
        get { return nodeID; }
        set { nodeID = value; }
    }

    public BlockModel CurrentBlock
    {
        get { return currentBlock; }
        set { currentBlock = value; }
    }

    public void GetNeighbours(List<Node> nodesList)
    {
        int foundAmount = 0;

        for (int x = (int)nodeID.x -1; x < (int)nodeID.x + 2; x++)
        {
            for (int y = (int)nodeID.y - 1; y < (int)nodeID.y + 2; y++)
            {
                if (x == nodeID.x && y == nodeID.y) continue; // Skip own cell.

                if (x == nodeID.x || y == nodeID.y) // Matchs adjacency
                {
                    Debug.Log("Found Adjacent Node");

                    Node neighbour = FindNeighbourID(x, y, nodesList);
                    if (neighbour != null && foundAmount < 4)
                    {
                        neighbourNodes.Add(neighbour);
                        foundAmount++;
                    }
                }
            }
        }
    }

    private Node FindNeighbourID(int currX, int currY, List<Node> nodesList)
    {
        for (int i = 0; i < nodesList.Count; i++)
        {
            Node currentNode = nodesList[i];
            if (currentNode.NodeID == new Vector2(currX, currY))
            {
                return currentNode;
            }
        }

        return null;
    }

    public void PaintVecinos()
    {
        foreach (Node node in neighbourNodes)
        {
            node.GetComponent<Image>().color = Color.red;
        }
    }
}
