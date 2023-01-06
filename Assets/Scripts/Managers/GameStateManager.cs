using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    public static GameStateManager Instance => instance;

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

    [Header("Player Units")]
    [SerializeField] private List<Piece> playerPieces;

    private bool isStateCoroutineRunning = false;

    public List<Piece> PlayerPieces => playerPieces;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        SetCurrentState(States.PlayerTurn);

        RecheckExistingAI();
    }


    private IEnumerator StartAutomaticTurnAI()
    {
        isStateCoroutineRunning = true;
        Debug.Log($"Enter start turn {currentState}");

        if (currentState == States.AITurn)
        {
            RecheckExistingAI();
            foreach (IAutoRunnableAI pieceAI in priorityOneAIList)
            {
                Debug.Log($"Drone executed behaviour!");
                pieceAI.AutoRunBehaviour();
                yield return new WaitUntil(() => pieceAI.IsAutoRunDone());
            }

            foreach (IAutoRunnableAI pieceAI in priorityTwoAIList)
            {
                Debug.Log($"Dreadnought executed behaviour!");
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

    public void RemoveDestroyedPlayerUnit(Piece playerPieceToRemove)
    {
        playerPieces.Remove(playerPieceToRemove);
    }

    private void RecheckExistingAI()
    {
        priorityOneAIList.Clear();
        priorityTwoAIList.Clear();
        priorityThreeAIList.Clear();

        foreach (Piece drone in drones)
        {
            if(drone != null)
            {
                priorityOneAIList.Add(drone.GetComponent<IAutoRunnableAI>());
           //     allPiecesAI.Add(drone);
            }
                
        }
           

        foreach (Piece dreadnought in dreadnoughts)
        {
            if(dreadnought != null)
            {
                priorityTwoAIList.Add(dreadnought.GetComponent<IAutoRunnableAI>());
           //     allPiecesAI.Add(dreadnought);
            }
               
        }
           

        foreach (Piece commandUnit in commandUnits)
        {
            if(commandUnit != null)
            {
                priorityThreeAIList.Add(commandUnit.GetComponent<IAutoRunnableAI>());
            //    allPiecesAI.Add(commandUnit);
            }
               
        }         
    }
}
