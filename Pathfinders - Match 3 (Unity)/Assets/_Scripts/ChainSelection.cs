using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSelection : MonoBehaviour
{
    [SerializeField] private bool startedChain;
    [SerializeField] private BlockModel.BLOCK_TYPE chainType;
    [SerializeField] private List<Node> chainedNodes;

    public void StartChain(Node currentNode, BlockModel.BLOCK_TYPE currentType)
    {
        startedChain = true;
        chainType = currentType;
        chainedNodes.Add(currentNode);
    }

    public void CheckBlockType(Node currentNode)
    {
        if (chainedNodes.Contains(currentNode))
        {
            chainedNodes[chainedNodes.Count - 1].image.color = Color.clear;
            chainedNodes.RemoveAt(chainedNodes.Count - 1);
        }

        if (!chainedNodes.Contains(currentNode) && startedChain == true && chainedNodes.Count > 0)
        {
            if (currentNode.NeighbourNodes.Contains(chainedNodes[chainedNodes.Count - 1]))
            {
                if (currentNode.CurrentBlock.BlockType == chainType)
                {
                    chainedNodes.Add(currentNode);
                    currentNode.image.color = Color.yellow;
                }
            }
        }


    }

    public void StopChain()
    {
        if (chainedNodes.Count >= GameManager.Instance.chainComboAmount)
        {
            RemoveFromChain();
        }

        else
        {
            RemovePaint();
        }

        chainedNodes.Clear();
    }

    public void RemoveFromChain()
    {
        foreach (var node in chainedNodes)
        {
            node.image.color = Color.clear;
            node.IsAir = true;
            node.CurrentBlock.DestroyBlock(true);
            node.CurrentBlock = null;
        }

        startedChain = false;
        chainedNodes.Clear();
        GameManager.Instance.UseTurn();
        GameManager.Instance.match3Grid.isFalling = true;
    }

    private void RemovePaint()
    {
        foreach (var item in chainedNodes)
        {
            item.image.color = Color.clear;
        }
    }
}
