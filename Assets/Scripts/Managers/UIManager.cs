using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button endPlayerTurnBtn;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI gameStatusText;

    private void Start()
    {
        endGamePanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameEventManager.OnPlayerWon += DisplayPlayerWonUI;
        GameEventManager.OnAIWon += DisplayPlayerLostUI;
    }

    private void OnDisable()
    {
        GameEventManager.OnPlayerWon -= DisplayPlayerWonUI;
        GameEventManager.OnAIWon -= DisplayPlayerLostUI;
    }

    public void SetActiveEndPlayerTurnButton(bool isActive) => endPlayerTurnBtn.gameObject.SetActive(isActive);

    private void EnableEndGameUI()
    {
        SetActiveEndPlayerTurnButton(false);
        endGamePanel.SetActive(true);
    }

    private void DisplayPlayerWonUI()
    {
        gameStatusText.text = "You won! Congratulations!";
        EnableEndGameUI();
    }

    private void DisplayPlayerLostUI()
    {
        gameStatusText.text = "You lost... But keep trying!";
        EnableEndGameUI();
    }
}
