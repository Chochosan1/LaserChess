using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum AI_TurnPriority { One, Two, Three }
    private static GameStateManager instance;
    public static GameStateManager Instance => instance;

    public enum States { PlayerTurn, AITurn }
    private States currentState;

    [Header("AI Units (automatically split in priority groups)")]
    [SerializeField] private List<Piece> aiPieces;
    private List<IAutoRunnableAI> priorityOneAIList = new List<IAutoRunnableAI>();
    private List<IAutoRunnableAI> priorityTwoAIList = new List<IAutoRunnableAI>();
    private List<IAutoRunnableAI> priorityThreeAIList = new List<IAutoRunnableAI>();

    [Header("Player Units")]
    [SerializeField] private List<Piece> playerPieces;

    private bool isStateCoroutineRunning = false;
    private bool gameEnded = false;

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

        SplitAIUnitsInPriorityGroups();
    }


    private IEnumerator StartAutomaticTurnAI()
    {
        isStateCoroutineRunning = true;
        Debug.Log($"Enter start turn {currentState}");

        if (currentState == States.AITurn)
        {
       //     RecheckExistingAI();
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

            foreach (IAutoRunnableAI pieceAI in priorityThreeAIList)
            {
                Debug.Log($"Command Unit executed behaviour!");
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
            foreach (Piece playerPiece in playerPieces)
                playerPiece.ResetTurnStatus();
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

        if (playerPieces.Count <= 0)
            gameEnded = true;
    }

    public void RemoveDestroyedAIUnit(IAutoRunnableAI aiPiece)
    {
        aiPieces.Remove(aiPiece.GetGameObject().GetComponent<Piece>());

        if (aiPiece.GetAITurnPriority() == AI_TurnPriority.One)
            priorityOneAIList.Remove(aiPiece);
        else if (aiPiece.GetAITurnPriority() == AI_TurnPriority.Two)
            priorityTwoAIList.Remove(aiPiece);
        else if (aiPiece.GetAITurnPriority() == AI_TurnPriority.Three)
            priorityThreeAIList.Remove(aiPiece);
    }

    private void SplitAIUnitsInPriorityGroups()
    {
        IAutoRunnableAI currentAutoRunnableAI;
        foreach(Piece aiPiece in aiPieces)
        {
            currentAutoRunnableAI = aiPiece.GetComponent<IAutoRunnableAI>();

            if (currentAutoRunnableAI.GetAITurnPriority() == AI_TurnPriority.One)
                priorityOneAIList.Add(currentAutoRunnableAI);
            else if (currentAutoRunnableAI.GetAITurnPriority() == AI_TurnPriority.Two)
                priorityTwoAIList.Add(currentAutoRunnableAI);
            else if (currentAutoRunnableAI.GetAITurnPriority() == AI_TurnPriority.Three)
                priorityThreeAIList.Add(currentAutoRunnableAI);
        }

        //foreach (Piece drone in drones)
        //{
        //    if (drone != null)
        //        priorityOneAIList.Add(drone.GetComponent<IAutoRunnableAI>());
        //}


        //foreach (Piece dreadnought in dreadnoughts)
        //{
        //    if (dreadnought != null)
        //        priorityTwoAIList.Add(dreadnought.GetComponent<IAutoRunnableAI>());
        //}


        //foreach (Piece commandUnit in commandUnits)
        //{
        //    if (commandUnit != null)
        //        priorityThreeAIList.Add(commandUnit.GetComponent<IAutoRunnableAI>());
        //}
    }
}
