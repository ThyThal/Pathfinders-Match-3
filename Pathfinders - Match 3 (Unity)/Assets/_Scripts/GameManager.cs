using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour
{

    // Game Manager Instance
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Starting Configuration")]
    [SerializeField] private Vector2 gridSize = new Vector2(10, 10);//
    [SerializeField] private int cellSize = 100;
    [SerializeField] private int maxStartingCombos = 3;//
    [SerializeField] private int chainComboAmount = 3;//
    [SerializeField] private float turnsAmount = 5;//
    [SerializeField] private int comboScore = 10;//

    [Header("Chains Configuration")]
    [SerializeField] private bool enableChainedCombos = true;//
    [SerializeField] private int maxChainedCombo = 2;//

    [Header("Components")]
    [SerializeField] public Match3Grid match3Grid;
    [SerializeField] public GameOverScreen conditionScreen;
    [SerializeField] public ChainSelection chainSelection;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioSource helpSource;
    [SerializeField] private RectTransform gridTransform;
    [SerializeField] private Transform particlesScale;
    [SerializeField] private RectTransform blockerTransform;
    [SerializeField] private AudioClip helpClip;//
    [SerializeField] private Text textTurns;//
    [SerializeField] private Text score;//

    [Header("Variables")]
    [SerializeField] private bool startingChain = true;//
    [SerializeField] private bool generatingRandomChains = false;//
    [SerializeField] private bool fallingBlocks = false;//
    [SerializeField] private bool generatingNewBlocks = false;//
    [SerializeField] private bool usedHelp = false;//
    [SerializeField] private int scoreTotal = 0;//

    // Original Values
    private int originalChainedCombos;//
    private float originalTurns;//
    private int originalCombos;//

    private void Start()
    {
        // Original Values
        originalChainedCombos = maxChainedCombo;
        originalCombos = maxStartingCombos;
        originalTurns = turnsAmount;

        textTurns.text = $"{TurnsAmount}";
        score.text = $"{ScoreTotal}";
    }

    /*
     * Properties
     */
    public Vector2 GridSize
    {
        get { return gridSize; }
    }
    public float CellSize
    {
        get { return cellSize; }
    }
    public int ChainComboAmount
    {
        get { return chainComboAmount; }
        set { chainComboAmount = value; }
    }
    public int MaxStartingCombos
    {
        get { return maxStartingCombos; }
        set { maxStartingCombos = value; }
    }
    public float TurnsAmount
    {
        get { return turnsAmount; }
        set { turnsAmount = value; }
    }
    public int ComboScore
    {
        get { return comboScore; }
    }
    public bool UsedHelp
    {
        get { return usedHelp; }
        set { usedHelp = value; }
    }
    public bool StartingChain
    {
        get { return startingChain; }
        set { startingChain = value; }
    }
    public bool GeneratingRandomChains
    {
        get { return generatingRandomChains; }
        set { generatingRandomChains = value; }
    }
    public bool FallingBlocks
    {
        get { return fallingBlocks; }
        set { fallingBlocks = value; }
    }
    public bool GeneratingNewBlocks
    {
        get { return generatingNewBlocks; }
        set { generatingNewBlocks = value; }
    }
    public int ScoreTotal
    {
        get { return scoreTotal; }
        set { scoreTotal = value; }
    }
    public bool EnableChainedCombos
    {
        get { return enableChainedCombos; }
    }
    public int MaxChainedCombo
    {
        get { return maxChainedCombo; }
        set { maxChainedCombo = value; }
    }


    /*
     * Methods
     */
    public void UseTurn()
    {
        MaxChainedCombo = originalChainedCombos;

        if (TurnsAmount > 0)
        {
            TurnsAmount--;
            textTurns.text = $"{TurnsAmount}";

            float percent = ((TurnsAmount) / (originalTurns)) * 100;
            if (percent < 25 || TurnsAmount <= 1)
            {
                audioSource.pitch = 1.2f;
            }
        }
    }
    public void AddScore(int amount)
    {
        ScoreTotal += amount;
        score.text = score.text = $"{scoreTotal}";
    }
    public void ResetGame()
    {
        // Variables
        maxChainedCombo = originalChainedCombos;
        maxStartingCombos = originalCombos;
        turnsAmount = originalTurns;

        StartingChain = true;
        ScoreTotal = 0;
        audioSource.pitch = 1;

        // UI
        textTurns.text = $"{TurnsAmount}";
        score.text = $"{ScoreTotal}";

        // Grid
        match3Grid.ResetGame();
        conditionScreen.HideMenu();
    }
    public void PlayHelpSound()
    {
        helpSource.PlayOneShot(helpClip);
    }
    public void SetGridBackgroundSize(Vector2 newSize)
    {
        // Background Size
        gridTransform.sizeDelta = newSize;
        blockerTransform.sizeDelta = newSize;

        // Particles Scale
        float defaultCellSize = 100;
        float scale = (cellSize * defaultCellSize) / 10000;
        Vector2 partScale = new Vector2(scale, scale);
        particlesScale.localScale = partScale;
    }

    /*
     * Exit Game
     */
    public void OnClickExit()
    {
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
        Application.Quit();
    }
}
