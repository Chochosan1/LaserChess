using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button endPlayerTurnBtn;
    [SerializeField] private GameObject endGamePanel;

    public void SetActiveEndPlayerTurnButton(bool isActive) => endPlayerTurnBtn.gameObject.SetActive(isActive);

    public void ShowEndGamePanel()
    {
        SetActiveEndPlayerTurnButton(false);
        endGamePanel.SetActive(true);
    }
}
