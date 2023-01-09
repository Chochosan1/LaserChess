using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Provides the necessary functionality/templates/fields for the final pieces that will be used on the grid/map. 
/// </summary>
public abstract class Piece : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected PieceStats stats;
    [SerializeField] protected float movementSpeed = 1f;
    [SerializeField] private LayerMask damagePiecesOnThisLayer;

    [Header("Displayable")]
    [SerializeField] private string pieceName = "";

    [Header("VFX")]
    [SerializeField] protected GameObject hitVFX;
    [SerializeField] protected GameObject destroyedVFX;

    protected int maxHitPoints, currentHitPoints;
    protected bool hasPlayedItsTurn = false; //has the piece logic already been executed this turn?

    /// <summary>The tile on which this piece is standing on.</summary>
    public GridTile StandingOnTile { get; set; }
    public int AttackPower => stats.AttackPower;

    /// <summary>This piece can damage only other pieces that are in this layer.</summary>
    public LayerMask DamagePiecesOnThisLayer => damagePiecesOnThisLayer;

    protected virtual void Start()
    {
        FindInitialStandingOnTile();

        StandingOnTile.MarkTileAsBlocked(this);

        maxHitPoints = currentHitPoints = stats.HitPoints;
    }

    public virtual void OnSelectedPiece() => GameEventManager.OnPieceSelectedByPlayer?.Invoke(pieceName);

    public virtual void OnDeselectedPiece() => GameEventManager.OnPieceDeselected?.Invoke();

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
        StandingOnTile.MarkTileAsFree();
        
        //VFX
        destroyedVFX.SetActive(true);
        destroyedVFX.transform.parent = null;
        Destroy(destroyedVFX, 2f);

        Destroy(this.gameObject);
    }

    /// <summary>Marks this piece as not having played this turn/round yet.</summary>
    public void ResetTurnStatus()
    {
        hasPlayedItsTurn = false;
    }

    //autofind the initial tile the piece is standing on (avoids manually finding and assigning it for all units)
    private void FindInitialStandingOnTile()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            if (hit.collider.TryGetComponent(out GridTile gridTileHitBelow))
            {
                StandingOnTile = gridTileHitBelow;
            }
        }
    }
}
