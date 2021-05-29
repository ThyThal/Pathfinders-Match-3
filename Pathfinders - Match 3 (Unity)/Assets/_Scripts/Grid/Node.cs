﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Node : MonoBehaviour
{
    [Header("Helpers")]
    [SerializeField] private bool isAir;
    [SerializeField] private BlockModel currentBlock;
    [SerializeField] private List<Node> neighbourNodes;
    [SerializeField] private Node fallingLocation;
    [SerializeField] private Vector2 nodeID;
    [SerializeField] private List<Node> chain;
    [SerializeField] private int minimumChainAmount = 3;

    [Header("Components")]
    [SerializeField] public Image image;

    /*
     * Events
     */

    /*
     * Properties
     */
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
    public bool IsAir
    {
        get { return isAir; }
        set { isAir = value; }
    }
    public Node FallingLocation
    {
        get { return fallingLocation; }
    }

    /*
     * Methods
     */
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
    public void CreateNewChain()
    {
        chain.Clear();
        if (IsAir == false)
        {
            chain.Add(this);
            CheckForChain(chain, 0);
            CheckChainSize(chain);
        }
    }
    private void CheckForChain(List<Node> chainList, int currentNode)
    {
        int currentNodeInChain = currentNode;

        foreach (var vecino in chainList[currentNodeInChain].NeighbourNodes)
        {
            if (vecino != null && vecino.currentBlock != null)
            {
                if (vecino.currentBlock.BlockType == chainList[0].currentBlock.BlockType)
                {
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
        if (chainList.Count >= minimumChainAmount)
        {
            RemoveBlocksFromChain(chainList);
            chain.Clear();
        }  
    }

    private void RemoveBlocksFromChain(List<Node> chainList)
    {
        foreach (var node in chainList)
        {
            node.isAir = true;
            node.CurrentBlock.DestroyBlock();
        }

        GameManager.Instance.match3Grid.isFalling = true;
    }
    public bool HasSpaceBelow()
    {
        for (int i = 0; i < neighbourNodes.Count; i++)
        {
            var currentNode = neighbourNodes[i];

            if (currentNode.NodeID.x == NodeID.x + 1 && currentNode.NodeID.y == NodeID.y)
            {
                if (currentNode.isAir == true)
                {
                    fallingLocation = currentNode;
                    return true;
                }
            }
        }

        return false;
    }
}
