using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private GameObject gameInfo;
    [SerializeField] private Image gameBoard;
    [SerializeField] private TextMeshProUGUI score;

    /*
     * Methods
     */
    public void ShowMenu()
    {
        gameInfo.SetActive(false);
        gameBoard.enabled = false;
        score.text = GameManager.Instance.ScoreTotal.ToString();
    }
    public void HideMenu()
    {
        gameInfo.SetActive(true);
        gameBoard.enabled = true;
        gameObject.SetActive(false);
    }
}
