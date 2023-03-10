using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager
{
    public delegate void OnGameEndedDelegated(string description);

    /// <summary>Invoked when the player wins the game. Carries a description of the win condition</summary>
    public static OnGameEndedDelegated OnPlayerWon;

    /// <summary>Invoked when the AI wins the game.Carries a description of the win condition</summary>
    public static OnGameEndedDelegated OnAIWon;

    public delegate void OnGameTurnStateChangedDelegate(GameStateManager.States currentState);
    /// <summary>Invoked when the current turn state changes.</summary>
    public static OnGameTurnStateChangedDelegate OnGameTurnStateChanged;

    public delegate void OnPieceSelectedByPlayerDelegate(string selectionInfo);
    /// <summary>Invoked when the player selects a piece.</summary>
    public static OnPieceSelectedByPlayerDelegate OnPieceSelectedByPlayer;

    public delegate void OnPieceDeselectedDelegate();
    /// <summary>Invoked when a piece gets deselected.</summary>
    public static OnPieceDeselectedDelegate OnPieceDeselected;
}
