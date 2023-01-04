using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Grunt : Piece
{
    //orthogonal paths
    private List<GridTile> currentTopTileRoute;
    private List<GridTile> currentBotTileRoute;
    private List<GridTile> currentRightTileRoute;
    private List<GridTile> currentLeftTileRoute;

    //attack
    private List<GridTile> currentAttackPath; //reuse the same list to probe for different attack paths (e.g the 4 diagonals until an enemy is found)
    private bool isEnemyFoundDuringProbing = false;

    private GridTile currentGridTileToMoveTo;
    private bool isMoving;
    private float step;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (!isMoving)
            return;

        step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentGridTileToMoveTo.transform.position.x, transform.position.y, currentGridTileToMoveTo.transform.position.z), step);

        //when the targeted tile has been reached
        if (Vector3.Distance(transform.position, new Vector3(currentGridTileToMoveTo.transform.position.x, transform.position.y, currentGridTileToMoveTo.transform.position.z)) < 0.01f)
        {
            standingOnTile = currentGridTileToMoveTo;
      //      standingOnTile.isBlocked = true;
            isMoving = false;

            Attack();
        }
    }

    public override void OnMoveCommand(GridTile selectedGridTileToMoveTo)
    {
        standingOnTile.MarkTileAsFree();

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        currentGridTileToMoveTo.MarkTileAsBlocked(this); //mark it as blocked immediately so that clicking on another unit won't show that tile as free while another unit is traveling to it
        isMoving = true;
    }

    public override void OnSelectedPiece()
    {
        currentTopTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 1, MapController.Directions.Top);
        currentBotTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 1, MapController.Directions.Bot);
        currentRightTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 1, MapController.Directions.Right);
        currentLeftTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 1, MapController.Directions.Left);

        foreach (GridTile gridTile in currentTopTileRoute)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentBotTileRoute)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentRightTileRoute)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentLeftTileRoute)
        {
            gridTile.ActivateTile();
        }
    }

    public override void OnDeselectedPiece()
    {
        foreach (GridTile gridTile in currentTopTileRoute)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentBotTileRoute)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentRightTileRoute)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentLeftTileRoute)
        {
            gridTile.DeactivateTile();
        }
    }

    protected override void Attack()
    {
        isEnemyFoundDuringProbing = false;
        currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.TopRight, true);

        if(!isEnemyFoundDuringProbing)
        {
            foreach (GridTile tile in currentAttackPath)
            {
             //   Debug.Log(tile.name);
                if (tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                    Debug.Log($"WILL SHOOT ON THE TOP RIGHT DIAGONAL TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    tile.BlockingTilePiece.TakeDamage(stats.AttackPower);
                    break;
                }
            }
        }
    }
}
