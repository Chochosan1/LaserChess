using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class controls the game state/flow. Provides a singleton for ease-of-use.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    private static GameStateManager instance;
    public static GameStateManager Instance => instance;

    public enum AI_TurnPriority { One, Two, Three }
    public enum States { PlayerTurn, AITurn }

    [Header("AI Units (automatically split in priority groups)")]
    [SerializeField] private List<Piece> aiPieces;

    [Header("Player Units")]
    [SerializeField] private List<Piece> playerPieces;

    //flow controlling vars
    private States currentState;
    private bool isStateCoroutineRunning = false;
    private bool gameEnded = false;
    private int commandUnitsLeftToDestroy = 0;

    //priority group lists
    private List<IAutoRunnableAI> priorityOneAIList = new List<IAutoRunnableAI>();
    private List<IAutoRunnableAI> priorityTwoAIList = new List<IAutoRunnableAI>();
    private List<IAutoRunnableAI> priorityThreeAIList = new List<IAutoRunnableAI>();

    public List<Piece> PlayerPieces => playerPieces;
    public States CurrentState => currentState;


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

        Debug.Log($"Enter {currentState}");

        //run the different priority lists of AI
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

        isStateCoroutineRunning = false;

        SetCurrentState(States.PlayerTurn);
    }

    /// <summary>Sets the current turn state.</summary>
    public void SetCurrentState(States stateToChangeTo)
    {
        //don't allow a new state to start if the previous hasn't finished
        if (isStateCoroutineRunning || gameEnded)
            return;

        currentState = stateToChangeTo;

        //on player's turn allow all player pieces to be played with again
        if (stateToChangeTo == States.PlayerTurn)
        {
            foreach (Piece playerPiece in playerPieces)
                playerPiece.ResetTurnStatus();
        }
        else //on AI's turn start running the automatic AI behaviours & deselect all currently selected player units if any
        {
            foreach (Piece playerPiece in playerPieces)
                playerPiece.OnDeselectedPiece();

            StartCoroutine(StartAutomaticTurnAI());
        }

        GameEventManager.OnGameTurnStateChanged?.Invoke(stateToChangeTo); //currently handled only by the UI manager to switch the UI
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

        //player lose condition
        if (playerPieces.Count <= 0)
        {
            GameEventManager.OnAIWon?.Invoke("All player units lost :(");
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

        //win condition 1: check if all AI pieces have been destroyed
        if (aiPieces.Count <= 0)
        {
            GameEventManager.OnPlayerWon?.Invoke("All AI units destroyed!");
            gameEnded = true;
            return;
        }

        //win condition 2: check if all command units have been destroyed
        if (aiPieceAutoRunnable.GetGameObject().GetComponent<CommandUnit>() != null)
        {
            commandUnitsLeftToDestroy--;

            if (commandUnitsLeftToDestroy <= 0)
            {
                GameEventManager.OnPlayerWon?.Invoke("All command units destroyed!");
                return;
            }
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
