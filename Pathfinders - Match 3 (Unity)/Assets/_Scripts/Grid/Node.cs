using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Node : MonoBehaviour
{
    [Header("Helpers")]
    [SerializeField] private bool isAir;
    [SerializeField] private BlockModel currentBlock;
    [SerializeField] private List<Node> neighbourNodes;
    [SerializeField] private Node fallingLocation;
    [SerializeField] private Vector2 nodeID;
    [SerializeField] private List<Node> chain;
    [SerializeField] private int minimumChainAmount;
    [SerializeField] private bool isPlayer;
    [SerializeField] private bool hasChain;

    [Header("Components")]
    [SerializeField] public Image image;

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
    public bool HasChain
    {
        get { return hasChain; }
        set { hasChain = value; }
    }

    /*
     * MonoBehaviour
     */
    private void Start()
    {
        minimumChainAmount = GameManager.Instance.ChainComboAmount;
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
    public void CreateNewChain(bool debugMode)
    {
        if (chain != null)
        {
            ClearChainList();
        }

        if (IsAir == false)
        {
            chain.Add(this);
            CheckForChain(chain, 0);

            if (GameManager.Instance.GeneratingRandomChains)
            {
                CheckChainSize(chain, minimumChainAmount - 1, debugMode);
            }

            else
            {
                CheckChainSize(chain, minimumChainAmount, debugMode);
            }
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

        }

        else
        {
            CheckForChain(chainList, currentNodeInChain + 1);
        }
        
    }
    private void CheckChainSize(List<Node> chainList, int chainSize, bool debugMode)
    {
        if (chainList.Count >= chainSize)
        {
            if (GameManager.Instance.StartingChain == true) // Primera Generacion de Grilla
            {
                RandomizeType();
            }

            if (GameManager.Instance.StartingChain == false)
            {
                if (GameManager.Instance.GeneratingRandomChains == true)
                {
                    if (chainSize != minimumChainAmount)
                    {
                        foreach (var node in chainList)
                        {
                            if (node.CurrentBlock != chainList[0])
                            {
                                int randomVecino = UnityEngine.Random.Range(0, node.NeighbourNodes.Count - 1);
                                Node vecino = node.NeighbourNodes[randomVecino];

                                vecino.CurrentBlock.BlockType = chainList[0].CurrentBlock.BlockType;
                            }
                        }

                        GameManager.Instance.MaxStartingCombos--;
                    }
                }

                else
                {
                    if (debugMode)
                    {
                        hasChain = true;
                        DebugPaintChain();
                    }

                    else
                    {
                        if (GameManager.Instance.EnableChainedCombos && GameManager.Instance.MaxChainedCombo > 0)
                        {
                            GameManager.Instance.MaxChainedCombo--;
                            ClearSuccessfulChain(chainList);
                        }
                    }
                }

            }

            ClearChainList();
        }
    }

    private void ClearSuccessfulChain(List<Node> chainList)
    {
        RemoveBlocksFromChain(chainList);
        ClearChainList();
    }

    private void RemoveBlocksFromChain(List<Node> chainList)
    {
        foreach (var node in chainList)
        {
            node.isAir = true;
            node.CurrentBlock.DestroyBlock(isPlayer);
            node.CurrentBlock = null;
        }

        ClearChainList();

        if (isPlayer)
        {
            GameManager.Instance.UseTurn();
            isPlayer = false;
        }

        GameManager.Instance.match3Grid.isFalling = true;
    }

    [ContextMenu("Check Space")]
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
                    ClearChainList();
                    return true;
                }
            }
        }



        return false;
    }

    public void ClearChainList()
    {
        chain.Clear();
    }
    private void RandomizeType()
    {
        List<int> blockTypes = new List<int> {0,1,2,3,4,5,6,7};
        List<int> usedTypes = new List<int>();
        List<int> freeTypes = blockTypes.Except(usedTypes).ToList();

        for (int i = 0; i < neighbourNodes.Count; i++) // Get all elements adyacent.
        {
            usedTypes.Add((int)neighbourNodes[i].CurrentBlock.BlockType);
        }

        var randomType = UnityEngine.Random.Range(0, freeTypes.Count);
        CurrentBlock.BlockType = (BlockModel.BLOCK_TYPE)randomType;

        CreateNewChain(false);
    }

    private void DebugPaintChain()
    {
        for (int i = 0; i < chain.Count; i++)
        {
            if (GameManager.Instance.UsedHelp == true)
            {
                chain[i].image.color = Color.gray; // Helping Paint.
            }
        }
    }

    public void IsPlayer()
    {
        isPlayer = true;
        CreateNewChain(false);
    }

    /*
     * CHAINLINK GAMEPLAY
     */

    public void StartChain()
    {
        GameManager.Instance.chainSelection.StartChain(this, currentBlock.BlockType);
        image.color = Color.blue; // Chain Start
    }

    public void StopChain()
    {
        GameManager.Instance.chainSelection.StopChain();
    }

    public void CheckChainBlockType()
    {
        GameManager.Instance.chainSelection.CheckBlockType(this);
    }

}
