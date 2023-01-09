using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    public static GameStateManager Instance => instance;

    public enum AI_TurnPriority { One, Two, Three }
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
    private int commandUnitsLeftToDestroy = 0;

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

    //runs all AI behaviours depending on the priorities and then grants control to the player
    private IEnumerator StartAutomaticTurnAI()
    {
        isStateCoroutineRunning = true;
        Debug.Log($"Enter start turn {currentState}");

        //run the different priority lists of AI
        if (currentState == States.AITurn)
        {
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
        if (isStateCoroutineRunning || gameEnded)
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

    /// <summary>Removes the passed player Piece from the list and checks if the player has lost.</summary> 
    public void RemoveDestroyedPlayerUnit(Piece playerPieceToRemove)
    {
        playerPieces.Remove(playerPieceToRemove);

        if (playerPieces.Count <= 0)
        {
            GameEventManager.OnAIWon?.Invoke();
            gameEnded = true;
        }
    }

    /// <summary>Removes the passed AI Piece from the list and checks if the player has won.</summary> 
    public void RemoveDestroyedAIUnit(IAutoRunnableAI aiPieceAutoRunnable)
    {
        aiPieces.Remove(aiPieceAutoRunnable.GetGameObject().GetComponent<Piece>());

        if (aiPieceAutoRunnable.GetAITurnPriority() == AI_TurnPriority.One)
            priorityOneAIList.Remove(aiPieceAutoRunnable);
        else if (aiPieceAutoRunnable.GetAITurnPriority() == AI_TurnPriority.Two)
            priorityTwoAIList.Remove(aiPieceAutoRunnable);
        else if (aiPieceAutoRunnable.GetAITurnPriority() == AI_TurnPriority.Three)
            priorityThreeAIList.Remove(aiPieceAutoRunnable);

        if (aiPieceAutoRunnable.GetGameObject().GetComponent<CommandUnit>() != null)
        {
            commandUnitsLeftToDestroy--;
            Debug.Log($"Command units to destroy: {commandUnitsLeftToDestroy}");
        }

        if (aiPieces.Count <= 0)
        {
            GameEventManager.OnPlayerWon?.Invoke();
            gameEnded = true;
        }
    }

    //determines which AI piece belongs to which AI priority group list
    private void SplitAIUnitsInPriorityGroups()
    {
        IAutoRunnableAI currentAutoRunnableAI;
        foreach (Piece aiPiece in aiPieces)
        {
            currentAutoRunnableAI = aiPiece.GetComponent<IAutoRunnableAI>();

            if (currentAutoRunnableAI.GetAITurnPriority() == AI_TurnPriority.One)
                priorityOneAIList.Add(currentAutoRunnableAI);
            else if (currentAutoRunnableAI.GetAITurnPriority() == AI_TurnPriority.Two)
                priorityTwoAIList.Add(currentAutoRunnableAI);
            else if (currentAutoRunnableAI.GetAITurnPriority() == AI_TurnPriority.Three)
                priorityThreeAIList.Add(currentAutoRunnableAI);

            if (aiPiece.GetComponent<CommandUnit>() != null)
            {
                commandUnitsLeftToDestroy++;
              
            }
        }

        Debug.Log($"Command units to destroy: {commandUnitsLeftToDestroy}");
    }
}
