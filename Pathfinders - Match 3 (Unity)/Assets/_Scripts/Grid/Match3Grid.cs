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
    [SerializeField] private Vector2 gridSize = new Vector2(5, 5);
    [SerializeField] private int cellSize = 100;
    [SerializeField] public bool isFalling;
    [SerializeField] private float fallingTimeAmount;
    private float originalFallTimer;

    private void Start()
    {
        originalFallTimer = fallingTimeAmount;
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
            }
        }
    }

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
                nodeComponent.NodeID = new Vector2(x,y);
                gridNodeArray.Add(node.GetComponent<Node>());
                nodeComponent.CurrentBlock = SpawnBlock(node.transform);

            }
        }

        GetNodeNeighbours();
    }

    private BlockModel SpawnBlock(Transform nodeTransform)
    {
        var node = Instantiate(blockPrefab, nodeTransform);
        return node.GetComponent<BlockModel>();
    }

    private void GetNodeNeighbours()
    {
        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            Node currentNode = gridNodeArray[i];
            currentNode.GetNeighbours(gridNodeArray);
        }
    }


    public void SearchFloating()
    {
        StartCoroutine(SearchFloatingBlocks());
    }

    IEnumerator SearchFloatingBlocks()
    {
        yield return new WaitForSeconds(fallingTimeAmount);

        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            Node currentNode = gridNodeArray[i];

            if (currentNode.HasSpaceBelow() && currentNode.IsAir == false)
            {
                currentNode.image.color = Color.red;
                currentNode.FallingLocation.image.color = Color.green;
                DoBlockFall(currentNode, currentNode.FallingLocation);
            }
        }
    }

    private void DoBlockFall(Node start, Node finish)
    {        
        if (start.CurrentBlock != null)
        {
            start.CurrentBlock.gameObject.transform.SetParent(finish.transform); // Sets new CurrentBlock Parent.
            finish.CurrentBlock = finish.GetComponentInChildren<BlockModel>();
            finish.CurrentBlock.GetComponent<RectTransform>().localPosition = Vector3.zero;
            finish.IsAir = false;
            start.IsAir = true;
            start.CurrentBlock = null;
            finish.image.color = Color.clear;
            start.image.color = Color.clear;
        }
    }
}
