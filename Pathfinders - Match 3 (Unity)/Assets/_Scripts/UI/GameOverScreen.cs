using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private GameObject gameInfo;
    [SerializeField] private Image gameBoard;
    [SerializeField] private TMPro.TextMeshProUGUI score;

    [ContextMenu("Hide Screen")]
    public void ShowMenu()
    {
        gameInfo.SetActive(false);
        gameBoard.enabled = false;
        score.text = GameManager.Instance.ScoreTotal.ToString();
    }

    [ContextMenu("Show Screen")]
    public void HideMenu()
    {
        gameInfo.SetActive(true);
        gameBoard.enabled = true;
        this.gameObject.SetActive(false);
        //GameManager.Instance.ResetGame();
    }
}
