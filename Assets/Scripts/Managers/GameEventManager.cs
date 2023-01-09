using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager
{
    public delegate void OnGameEndedDelegated();
    public static OnGameEndedDelegated OnPlayerWon;
    public static OnGameEndedDelegated OnAIWon;
}
