using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum States { PlayerTurn, AITurn }
    private States currentState;

    [Header("AI Drones")]
    [SerializeField] private List<Piece> drones;
    private List<IAutoRunnableAI> priorityOneAIList = new List<IAutoRunnableAI>();

    [Header("AI Dreadnoughts")]
    [SerializeField] private List<Piece> dreadnoughts;
    private List<IAutoRunnableAI> priorityTwoAIList = new List<IAutoRunnableAI>();

    [Header("AI Command Units")]
    [SerializeField] private List<Piece> commandUnits;
    private List<IAutoRunnableAI> priorityThreeAIList = new List<IAutoRunnableAI>();

    private bool isStateCoroutineRunning = false;

    void Start()
    {
        SetCurrentState(States.PlayerTurn);

        foreach (Piece drone in drones)
            priorityOneAIList.Add(drone.GetComponent<IAutoRunnableAI>());

        foreach (Piece dreadnought in dreadnoughts)
            priorityTwoAIList.Add(dreadnought.GetComponent<IAutoRunnableAI>());

        foreach (Piece commandUnit in commandUnits)
            priorityThreeAIList.Add(commandUnit.GetComponent<IAutoRunnableAI>());
    }


    private IEnumerator StartAutomaticTurnAI()
    {
        isStateCoroutineRunning = true;
        Debug.Log($"Enter start turn {currentState}");

        if (currentState == States.AITurn)
        {
            foreach (IAutoRunnableAI pieceAI in priorityOneAIList)
            {
                Debug.Log($"Drone executed behaviour!");
                pieceAI.AutoRunBehaviour();
                yield return new WaitUntil(() => pieceAI.IsAutoRunDone());
            }
        }

        Debug.Log($"Exit start turn {currentState}");
        isStateCoroutineRunning = false;

        SetCurrentState(States.PlayerTurn);
    }

    private void SetCurrentState(States stateToChangeTo)
    {
        //don't allow a new state to start if the previous hasn't finished
        if (isStateCoroutineRunning)
            return;

        Debug.Log($"Set current state {currentState}");
        currentState = stateToChangeTo;

        if (stateToChangeTo == States.PlayerTurn)
        {
            GetComponent<UIManager>().SetActiveEndPlayerTurnButton(true);
        }
        else
        {
            GetComponent<UIManager>().SetActiveEndPlayerTurnButton(false);
            StartCoroutine(StartAutomaticTurnAI());
        }   
    }

    //for the UI button that ends the player's turn
    public void EndPlayerState()
    {
        SetCurrentState(States.AITurn);
    }
}
