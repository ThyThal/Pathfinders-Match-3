using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject blockPrefab;

    [Header("Customize")]
    [SerializeField] private Vector2 gridSize = new Vector2(5, 5);
    [SerializeField] private int cellSize = 100;

    private void Start()
    {
        rectTransform.sizeDelta = new Vector2(gridSize.x * cellSize, gridSize.y * cellSize);
        SpawnBlocks();
    }

    private void SpawnBlocks()
    {
        float totalBlockAmount = (gridSize.x * gridSize.y);

        for (int i = 0; i < totalBlockAmount; i++)
        {
            Instantiate(blockPrefab, gridLayoutGroup.transform);
        }
    }
}
