using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Turn States UI")]
    [SerializeField] private Button endPlayerTurnBtn;

    [Header("End Game UI")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private Button restartGameBtn;
    [SerializeField] private TextMeshProUGUI gameStatusText;

    private void Start()
    {
        endGamePanel.SetActive(false);
    }

    //subscribe to the events
    private void OnEnable()
    {
        GameEventManager.OnPlayerWon += DisplayPlayerWonUI;
        GameEventManager.OnAIWon += DisplayPlayerLostUI;
        GameEventManager.OnGameTurnStateChanged += ChangeUIBasedOnTurnState;
    }

    //unsubscribe from the events
    private void OnDisable()
    {
        GameEventManager.OnPlayerWon -= DisplayPlayerWonUI;
        GameEventManager.OnAIWon -= DisplayPlayerLostUI;
        GameEventManager.OnGameTurnStateChanged -= ChangeUIBasedOnTurnState;
    }

    //for the UI button that ends the player's turn
    public void EndPlayerState()
    {
        GameStateManager.Instance.SetCurrentState(GameStateManager.States.AITurn);
    }


    //for the UI buttons that load levels (e.g restart, next level)
    public void LoadLevel(int levelIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelIndex);
    }

    private void SetActiveEndPlayerTurnButton(bool isActive) => endPlayerTurnBtn.gameObject.SetActive(isActive);

    private void ChangeUIBasedOnTurnState(GameStateManager.States currentState)
    {
        if (currentState == GameStateManager.States.PlayerTurn)
            SetActiveEndPlayerTurnButton(true);
        else
            SetActiveEndPlayerTurnButton(false);
    }

    private void EnableEndGameUI()
    {
        SetActiveEndPlayerTurnButton(false);
        endGamePanel.SetActive(true);
    }

    private void DisplayPlayerWonUI(string description)
    {
        gameStatusText.text = $"You won! {description} Congratulations!";
        EnableEndGameUI();
    }

    private void DisplayPlayerLostUI(string description)
    {
        gameStatusText.text = $"You lost... {description} Don't give up, though!";
        EnableEndGameUI();
    }
}
