using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected PieceStats stats;

    protected int maxHitPoints, currentHitPoints;

    public GridTile standingOnTile;
    public float movementSpeed = 1f;
    public LayerMask damagePiecesOnThisLayer;

    protected virtual void Start()
    {
        standingOnTile.MarkTileAsBlocked(this);

        maxHitPoints = currentHitPoints = stats.HitPoints;
    }

    public abstract void OnSelectedPiece();


    public abstract void OnDeselectedPiece();


    public abstract void OnMoveCommand(GridTile selectedGridTileToMoveTo);

    protected abstract void Attack();

    public virtual void TakeDamage(int damageToTake)
    {
        currentHitPoints -= damageToTake;
        Debug.Log($"I just received {damageToTake} damage.");

        if (currentHitPoints <= 0)
            Destroy(this.gameObject);
    }
}
