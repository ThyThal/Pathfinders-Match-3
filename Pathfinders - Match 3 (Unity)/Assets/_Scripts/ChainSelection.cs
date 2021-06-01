using System.Collections.Generic;
using UnityEngine;

public class ChainSelection : MonoBehaviour
{
    [Header("Main Info")]
    [SerializeField] private bool startedChain;
    [SerializeField] private BlockModel.BLOCK_TYPE chainType;
    [SerializeField] private List<Node> chainedNodes;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chainAudio;
    [SerializeField] private AudioClip removeAudio;
    [SerializeField] private AudioClip addAudio;
    [SerializeField] private AudioClip errorAudio;

    /*
     * Methods
     */
    public void StartChain(Node currentNode, BlockModel.BLOCK_TYPE currentType)
    {
        startedChain = true;
        chainType = currentType;
        chainedNodes.Add(currentNode);
        PlayAudio(addAudio);
    }
    public void CheckBlockType(Node currentNode)
    {
        if (chainedNodes.Contains(currentNode))
        {
            chainedNodes[chainedNodes.Count - 1].image.color = Color.clear; // Chain Remove Color.
            chainedNodes.RemoveAt(chainedNodes.Count - 1);
            PlayAudio(removeAudio);
        }

        if (!chainedNodes.Contains(currentNode) && startedChain == true && chainedNodes.Count > 0)
        {
            if (currentNode.NeighbourNodes.Contains(chainedNodes[chainedNodes.Count - 1]))
            {
                if (currentNode.CurrentBlock.BlockType == chainType)
                {
                    chainedNodes.Add(currentNode);
                    PlayAudio(addAudio);
                    currentNode.image.color = Color.yellow; // Chain Added Block.
                }
            }
        }
    }
    public void StopChain()
    {
        if (chainedNodes.Count >= GameManager.Instance.ChainComboAmount)
        {
            PlayAudio(chainAudio);
            RemoveFromChain();
        }

        else
        {
            RemovePaint();
            PlayAudio(errorAudio);
        }

        chainedNodes.Clear();
    }
    public void RemoveFromChain()
    {
        foreach (var node in chainedNodes)
        {
            //node.image.color = Color.clear;
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
            item.image.color = Color.clear; // Chain Failed.
        }
    }
    private void PlayAudio(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
