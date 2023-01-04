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


    private IEnumerator StartTurn()
    {
        if (currentState == States.AITurn)
        {
            foreach (IAutoRunnableAI pieceAI in priorityOneAIList)
            {
                pieceAI.AutoRunBehaviour();
                yield return new WaitUntil(() => pieceAI.IsAutoRunDone());
            }

            SetCurrentState(States.PlayerTurn);
        }
    }

    private void SetCurrentState(States stateToChangeTo)
    {
        currentState = stateToChangeTo;

        if (stateToChangeTo == States.PlayerTurn)
        {
            GetComponent<UIManager>().SetActiveEndPlayerTurnButton(true);
        }
        else
        {
            GetComponent<UIManager>().SetActiveEndPlayerTurnButton(false);
        }

        Debug.Log($"Starting turn {currentState}");
        StartCoroutine(StartTurn());
    }

    //for the UI button that ends the player's turn
    public void EndPlayerState()
    {
        SetCurrentState(States.AITurn);
    }
}
