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
    public List<Node> NeighbourNodes
    {
        get { return neighbourNodes; }
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

    public void CreateNewChain()
    {
        List<Node> chain = new List<Node>();
        chain.Add(this);
        this.GetComponent<Image>().color = Color.green;

        CheckForChain(chain, 0);
        CheckChainSize(chain);
    }

    private void CheckForChain(List<Node> chainList, int currentNode)
    {
        int currentNodeInChain = currentNode;

        foreach (var vecino in chainList[currentNodeInChain].NeighbourNodes)
        {
            if (vecino != null)
            {
                if (vecino.currentBlock.BlockType == chainList[0].currentBlock.BlockType)
                {
                    vecino.GetComponent<Image>().color = Color.blue;

                    for (int i = 0; i < chainList.Count; i++)
                    {
                        if (!chainList.Contains(vecino))
                        {
                            chainList.Add(vecino);
                        }
                    }
                }
            }
        }

        if (currentNodeInChain >= chainList.Count - 1)
        {
            Debug.Log("STOP");
        }

        else
        {
            CheckForChain(chainList, currentNodeInChain + 1);
        }
        
    }

    private void CheckChainSize(List<Node> chainList)
    {
        if (chainList.Count >= 3)
        {
            RemoveBlocksFromChain(chainList);
        }
    }

    private void RemoveBlocksFromChain(List<Node> chainList)
    {
        foreach (var block in chainList)
        {
            block.GetComponent<Image>().color = Color.red;
            block.CurrentBlock.DestroyBlock();
        }
    }
}
