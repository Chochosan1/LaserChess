using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Inherited by a Piece that should have some extra functionality to allow it to run separately (not controlled by the player). For example - Drone, Dreadnought, CommandUnit.
/// </summary>
public interface IAutoRunnableAI
{
    /// <summary>The behaviour the AI will auto-execute.</summary>
    public void AutoRunBehaviour();

    /// <summary>Has the auto-execute behaviour finished running?</summary>
    public bool IsAutoRunDone();

    /// <summary>Returns the turn priority of this AI unit.</summary>
    public GameStateManager.AI_TurnPriority GetAITurnPriority();

    /// <summary>Returns the gameObject this interface instance serves.</summary>
    public GameObject GetGameObject();
}
