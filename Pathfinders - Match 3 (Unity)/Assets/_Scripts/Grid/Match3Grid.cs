using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Match3Grid : MonoBehaviour
{
    public List<Node> gridNodeArray;

    [Header("Components")]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private RectTransform rectTransform;

    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject blockPrefab;

    [Header("Customize")]
    [SerializeField] private int cellSize = 100;
    [SerializeField] public bool isFalling;
    [SerializeField] private float fallingTimeAmount;
    [SerializeField] private List<Node> updatedFalling;
    private Vector2 gridSize;
    private float originalFallTimer;
    private int originalStartingCombos;

    [SerializeField] private List<Node> availableAux = new List<Node>();

    private void Start()
    {
        gridSize = GameManager.Instance.gridSize;
        originalFallTimer = fallingTimeAmount;
        originalStartingCombos = GameManager.Instance.maximumStartingCombos;
        rectTransform.sizeDelta = new Vector2(gridSize.x * cellSize, gridSize.y * cellSize);
        SpawnNodes();
    }

    private void Update()
    {
        if (isFalling)
        {
            fallingTimeAmount -= Time.deltaTime;
            if (fallingTimeAmount > 0)
            {
                SearchFloating();
            }

            else
            {
                isFalling = false;
                fallingTimeAmount = originalFallTimer;

                DoBlockRegeneration();
            }
        }
    }

    /*
     * Starting Grid Setup  
     */
    private void SpawnNodes()
    {
        //float totalBlockAmount = (gridSize.x * gridSize.y);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var node = Instantiate(nodePrefab, gridLayoutGroup.transform);
                node.name = "Node: " + $"[{x},{y}]";

                Node nodeComponent = node.GetComponent<Node>();
                nodeComponent.NodeID = new Vector2(x, y);
                gridNodeArray.Add(node.GetComponent<Node>());
                nodeComponent.CurrentBlock = SpawnBlock(node.transform);

            }
        }

        GetNodeNeighbours();
        CheckChains();
    }
    private void GetNodeNeighbours()
    {
        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            Node currentNode = gridNodeArray[i];
            currentNode.GetNeighbours(gridNodeArray);
        }
    }
    private BlockModel SpawnBlock(Transform nodeTransform)
    {
        var node = Instantiate(blockPrefab, nodeTransform);
        return node.GetComponent<BlockModel>();
    }

    public void CheckChains()
    {
        StartCoroutine(CheckStartingChains());
        availableAux = new List<Node>(gridNodeArray);
    }
    IEnumerator CheckStartingChains()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = gridNodeArray.Count - 1; i >= 0; i--)
        {
            gridNodeArray[i].CreateNewChain(false); // Check for chains and remove them.
        }

        GameManager.Instance.startingChain = false;
        GenerateRandomChains();
    }

    [ContextMenu("RANDOMIZE")]
    private void GenerateRandomChains()
    {
        GameManager.Instance.generatingRandomChains = true;

        if (GameManager.Instance.maximumStartingCombos > 0 && availableAux.Count > 0)
        {
            var randomInt = UnityEngine.Random.Range(0, availableAux.Count - 1);
            Node randomNode = availableAux[randomInt];
            randomNode.CreateNewChain(false);
            availableAux.Remove(randomNode);

            GenerateRandomChains();
        }

        if (GameManager.Instance.maximumStartingCombos <= 0)
        {
            GameManager.Instance.generatingRandomChains = false;
        }



    }



    /*
     * Main Gameplay Methods
     */

    public void SearchFloating() // Used after user input.
    {
        StartCoroutine(SearchFloatingBlocks());
    }
    IEnumerator SearchFloatingBlocks()
    {
        yield return new WaitForSeconds(fallingTimeAmount);

        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            Node currentNode = gridNodeArray[i];
            currentNode.image.color = Color.clear;
            if (currentNode.HasSpaceBelow() && currentNode.IsAir == false)
            {
                DoBlockFall(currentNode, currentNode.FallingLocation);
            }
        }
    }
    private void DoBlockFall(Node start, Node finish)
    {
        start.ClearChainList();

        if (start.CurrentBlock != null)
        {
            start.CurrentBlock.gameObject.transform.SetParent(finish.transform); // Sets new CurrentBlock Parent.
            finish.CurrentBlock = finish.GetComponentInChildren<BlockModel>();
            finish.CurrentBlock.GetComponent<RectTransform>().localPosition = Vector3.zero;
            finish.IsAir = false;
            start.IsAir = true;
            start.CurrentBlock = null;

            updatedFalling.Add(finish);
        }
    }

    public void FindNewChains() // Used after falling
    {
        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            gridNodeArray[i].CreateNewChain(false);
        }
    }

    private void DoBlockRegeneration()
    {
        StartCoroutine(RegenerateBlocks());
    }

    IEnumerator RegenerateBlocks()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            var currentNode = gridNodeArray[i];

            if (currentNode.CurrentBlock == null)
            {
                currentNode.CurrentBlock = SpawnBlock(currentNode.transform);
                currentNode.IsAir = false;
                updatedFalling.Add(currentNode);
            }
        }

        CheckUpdatedNodes();
    }


    private void CheckUpdatedNodes()
    {
        if (updatedFalling.Count > 0)
        {
            for (int i = 0; i < updatedFalling.Count; i++)
            {
                //updatedFalling[i].image.color = Color.yellow;
                StartCoroutine(UpdateFallChain(updatedFalling[i]));
            }
        }

        StartCoroutine(FindFallingChains());
        updatedFalling.Clear();
    }

    IEnumerator UpdateFallChain(Node node)
    {
        yield return new WaitForSeconds(0.1f);
        node.CreateNewChain(false);
    }

    IEnumerator FindFallingChains()
    {
        yield return new WaitForSeconds(0.1f);
        FindChains();
    }

    /*
     * Debug Buttons
     */
    public void FindChains()
    {
        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            gridNodeArray[i].image.color = Color.clear;
            gridNodeArray[i].CreateNewChain(true);
        }


    }

    [ContextMenu("Check No Chains")]
    private void CheckForNoChains()
    {
        bool foundChain = false;

        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            gridNodeArray[i].HasChain = false;

            if (foundChain == false)
            {
                gridNodeArray[i].CreateNewChain(true);
                foundChain = gridNodeArray[i].HasChain;
            }
        }

        if (foundChain)
        {
            Debug.Log("Found Chain");
        }

        if (!foundChain)
        {
            Debug.Log("Game Over");
        }
    }
}
