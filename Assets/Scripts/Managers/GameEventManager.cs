using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager
{
    public delegate void OnGameEndedDelegated(string description);

    /// <summary>Invoked when the player wins the game.</summary>
    public static OnGameEndedDelegated OnPlayerWon;

    /// <summary>Invoked when the AI wins the game.</summary>
    public static OnGameEndedDelegated OnAIWon;
}
