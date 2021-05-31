using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Customization")]
    [SerializeField] public Vector2 gridSize = new Vector2(10, 10);
    [SerializeField] public int chainComboAmount = 3;
    [SerializeField] public int maximumStartingCombos = 3;
    [SerializeField] public int turnsAmount = 5;
    [SerializeField] public int comboScore = 10;

    [Header("Extras")]
    [SerializeField] public Match3Grid match3Grid;
    [SerializeField] public GameOverScreen conditionScreen;
    [SerializeField] public ChainSelection chainSelection;
    [SerializeField] public bool startingChain;
    [SerializeField] public bool generatingRandomChains;
    [SerializeField] public bool fallingBlocks;
    [SerializeField] public bool generatingNewBlocks;

    [SerializeField] private Text textTurns;
    [SerializeField] private Text score;
    [SerializeField] public int scoreTotal;
    private int originalTurns;
    private int originalCombos;

    private void Start()
    {
        originalCombos = maximumStartingCombos;
        originalTurns = turnsAmount;
        textTurns.text = $"{turnsAmount}";
        score.text = $"{0}";
    }


    public void UseTurn()
    {
        if (turnsAmount > 0)
        {
            turnsAmount--;
            textTurns.text = $"{turnsAmount}";
        }
    }

    public void AddScore(int amount)
    {
        scoreTotal += amount;
        score.text = score.text = $"{scoreTotal}";
    }

    [ContextMenu("Reset Game")]
    public void ResetGame()
    {
        // Variables
        startingChain = true;
        maximumStartingCombos = originalCombos;
        turnsAmount = originalTurns;
        scoreTotal = 0;

        // UI
        textTurns.text = $"{turnsAmount}";
        score.text = score.text = $"{scoreTotal}";

        // Grid
        match3Grid.ResetGame();
        conditionScreen.HideMenu();
    }
}
