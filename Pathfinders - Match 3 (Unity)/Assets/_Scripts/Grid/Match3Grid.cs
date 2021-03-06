using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Match3Grid : MonoBehaviour
{
    public List<Node> gridNodeArray;

    [Header("Components")]
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject screenBlocker;
    [SerializeField] private BlockScreen blockScreen;

    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject blockPrefab;

    [Header("Customize")]
    [SerializeField] private bool isFalling;//
    [SerializeField] private float fallingTimeAmount;
    [SerializeField] private List<Node> updatedFalling;

    [Header("Player Settings")]
    [SerializeField] private bool playerTurn;
    [SerializeField] private float playerTurnTimer = 1f;
    [SerializeField] private float blockScreenTimer = 1f;
    [SerializeField] private bool endTurn = false;
    [SerializeField] private List<Node> availableAux = new List<Node>();

    private Vector2 gridSize;
    private float originalFallTimer;
    private float originalBlockTimer;

    /*
     * MonoBehaviour
     */
    private void Start()
    {
        originalFallTimer = fallingTimeAmount;
        originalBlockTimer = blockScreenTimer;
        var cellSize = GameManager.Instance.CellSize;
        gridSize = GameManager.Instance.GridSize;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        rectTransform.sizeDelta = new Vector2(gridSize.x * cellSize, gridSize.y * cellSize);
        GameManager.Instance.SetGridBackgroundSize(new Vector2(20, 20) + rectTransform.sizeDelta);
        SpawnNodes();
    }
    private void Update()
    {
        if (isFalling)
        {
            blockScreen.StartParticles();

            playerTurn = false;
            blockScreenTimer = originalBlockTimer; // Reset Screen Block Timer.

            fallingTimeAmount -= Time.deltaTime;
            if (fallingTimeAmount > 0)
            {
                TriggerSearchFloatingBlocks();
            }

            else
            {
                isFalling = false;
                fallingTimeAmount = originalFallTimer;

                DoBlockRegeneration();
            }
        }

        if (!isFalling && !playerTurn)
        {
            blockScreenTimer -= Time.deltaTime;
            screenBlocker.SetActive(true);

            if (blockScreenTimer <= 0)
            {
                blockScreen.StopParticles();
                playerTurn = true;
                endTurn = true;
            }
        }

        if (!isFalling && endTurn == true)
        {
            endTurn = false;
            CheckForLose();
        }
    }

    /*
     * Properties
     */
    public bool IsFalling
    {
        get { return isFalling; }
        set { isFalling = value; }
    }

    /*
     * Enumerators
     */
    IEnumerator CheckStartingChains()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = gridNodeArray.Count - 1; i >= 0; i--)
        {
            gridNodeArray[i].CreateNewChain(false); // Check for chains and remove them.
        }

        GameManager.Instance.StartingChain = false;
        GenerateRandomChains();
    }
    IEnumerator SearchFloatingBlocks()
    {
        yield return new WaitForSeconds(fallingTimeAmount);
        GameManager.Instance.UsedHelp = false;

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
    IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        //Debug.Log("Game Over");
        GameManager.Instance.conditionScreen.gameObject.SetActive(true);
        GameManager.Instance.conditionScreen.ShowMenu();
        GameManager.Instance.audioSource.pitch = 1;
        DeleteBlocks();
    }

    /*
     * Methods Enumerators
     */
    public void TriggerCheckStartingChains() // Enumerator Trigger.
    {
        StartCoroutine(CheckStartingChains());
        availableAux = new List<Node>(gridNodeArray);
    }
    public void TriggerSearchFloatingBlocks() // Enumerator Trigger.
    {
        StartCoroutine(SearchFloatingBlocks());
    }
    private void DoBlockRegeneration() // Enumerator Trigger.
    {
        StartCoroutine(RegenerateBlocks());
    }
    private void CheckUpdatedNodes() // Enumerator Trigger.
    {
        if (updatedFalling.Count > 0)
        {
            for (int i = 0; i < updatedFalling.Count; i++)
            {
                //updatedFalling[i].image.color = Color.yellow;
                StartCoroutine(UpdateFallChain(updatedFalling[i])); // Enumerator Trigger.
            }
        }

        StartCoroutine(FindFallingChains()); // Enumerator Trigger.
        updatedFalling.Clear();
    }
    private void GameOver() // Enumerator Trigger.
    {

        StartCoroutine(GameOverRoutine());
    }

    /*
     * Methods
     */
    private BlockModel SpawnBlock(Transform nodeTransform)
    {
        var node = Instantiate(blockPrefab, nodeTransform);
        return node.GetComponent<BlockModel>();
    }
    private void SpawnNodes()
    {
        for (int x = 0; x < GameManager.Instance.GridSize.y; x++)
        {
            for (int y = 0; y < GameManager.Instance.GridSize.x; y++)
            {
                var node = Instantiate(nodePrefab, gridLayoutGroup.transform);
                node.name = "Node: " + $"[{x},{y}]";
                Node nodeComponent = node.GetComponent<Node>();
                nodeComponent.NodeID = new Vector2(x, y);
                gridNodeArray.Add(node.GetComponent<Node>());

                // Spawn Block
                nodeComponent.CurrentBlock = SpawnBlock(node.transform);

            }
        }

        GetNodeNeighbours();
        TriggerCheckStartingChains();
    }
    private void GetNodeNeighbours()
    {
        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            Node currentNode = gridNodeArray[i];
            currentNode.GetNeighbours(gridNodeArray);
        }
    }
    private void GenerateRandomChains()
    {
        GameManager.Instance.GeneratingRandomChains = true;

        if (GameManager.Instance.MaxStartingCombos > 0 && availableAux.Count > 0)
        {
            var randomInt = UnityEngine.Random.Range(0, availableAux.Count - 1);
            Node randomNode = availableAux[randomInt];
            randomNode.CreateNewChain(false);
            availableAux.Remove(randomNode);

            GenerateRandomChains();
        }

        if (GameManager.Instance.MaxStartingCombos <= 0)
        {
            GameManager.Instance.GeneratingRandomChains = false;
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
    public void FindNewChains()
    {
        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            gridNodeArray[i].CreateNewChain(false);
        }
    }
    public void PaintRandomChain()
    {
        GameManager.Instance.UsedHelp = true;

        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            gridNodeArray[i].HasChain = false;
            gridNodeArray[i].CreateNewChain(true);
        }
    }
    public void FindChains()
    {
        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            gridNodeArray[i].CreateNewChain(true);
        }
    }
    private void CheckForLose()
    {
        bool foundChain = false;

        if (GameManager.Instance.TurnsAmount > 0)
        {
            for (int i = 0; i < gridNodeArray.Count; i++)
            {
                
                gridNodeArray[i].HasChain = false;

                if (foundChain == false)
                {
                    gridNodeArray[i].CreateNewChain(true);
                    foundChain = gridNodeArray[i].HasChain;
                }
            }

            if (!foundChain)
            {
                //Debug.Log("No Quedan Cadenas");
                GameOver();
            }
        }

        if (GameManager.Instance.TurnsAmount <= 0)
        {
            //Debug.Log("No Hay Mas Turnos");
            GameOver();
        }
    }
    private void DeleteBlocks()
    {
        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            gridNodeArray[i].CurrentBlock.DestroyBlock(false);
        }
    }
    public void ResetGame()
    {
        // Timers
        fallingTimeAmount = originalFallTimer;
        blockScreenTimer = originalBlockTimer;

        // Bools
        endTurn = false;

        // Spawning
        //SpawnNodes();
        ResetBlocks();
    }    
    private void ResetBlocks()
    {
        for (int i = 0; i < gridNodeArray.Count; i++)
        {
            var currentNode = gridNodeArray[i];
            currentNode.CurrentBlock = SpawnBlock(currentNode.transform);
            currentNode.IsAir = false;
        }

        TriggerCheckStartingChains();
    }
}
