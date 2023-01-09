using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected PieceStats stats;

    [Header("VFX")]
    [SerializeField] protected GameObject hitVFX;
    [SerializeField] protected GameObject destroyedVFX;

    protected int maxHitPoints, currentHitPoints;
    protected bool hasPlayedItsTurn = false; //has the piece logic already been executed this turn?

    public GridTile standingOnTile;
    public float movementSpeed = 1f;
    public LayerMask damagePiecesOnThisLayer;

    public int AttackPower => stats.AttackPower;

    protected virtual void Start()
    {
        //autofind the initial tile the piece is standing on (avoids manually finding and assigning it for all units)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            if (hit.collider.TryGetComponent(out GridTile gridTileHitBelow))
            {
                standingOnTile = gridTileHitBelow;
            }
        }

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

        //reset the hit VFX
        hitVFX.SetActive(false);
        hitVFX.SetActive(true);

        if (currentHitPoints <= 0)
        {
            Die();
        }       
    }

    protected virtual void Die()
    {
        standingOnTile.MarkTileAsFree();
        
        //VFX
        destroyedVFX.SetActive(true);
        destroyedVFX.transform.parent = null;
        Destroy(destroyedVFX, 2f);

        Destroy(this.gameObject);
    }

    public void ResetTurnStatus()
    {
        hasPlayedItsTurn = false;
    }
}
