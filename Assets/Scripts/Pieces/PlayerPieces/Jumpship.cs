using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserChess.Utilities;
public sealed class Jumpship : Piece
{
    //movement
    private List<GridTile> allPathsUsedTiles;
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
        allPathsUsedTiles = new List<GridTile>();
        List<GridTile> currentMovePath = new List<GridTile>();

        //finds the end point for each knight pattern and activates the tile (ready to be selected by the player); 
        foreach (int currKnightPattern in System.Enum.GetValues(typeof(MapController.KnightPattern)))
        {
            currentMovePath = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, (MapController.KnightPattern)currKnightPattern);

            foreach (GridTile tile in currentMovePath)
            {
                tile.ActivateTile();
                allPathsUsedTiles.Add(tile); //store all tiles from all paths here so they can get deactivated later
            }
        }
    }

    public override void OnDeselectedPiece()
    {
        foreach (GridTile tile in allPathsUsedTiles)
            tile.DeactivateTile();
    }

    protected override void Die()
    {
        GameStateManager.Instance.RemoveDestroyedPlayerUnit(this);
        base.Die();
    }

    //jumpships should damage all enemy pieces in the 4 orthogonally adjacent spaces simultaneously
    protected override void Attack()
    {
        if (standingOnTile.topNeighbour != null && standingOnTile.topNeighbour.BlockingTilePiece != null && LayerUtilities.IsObjectInLayer(standingOnTile.topNeighbour.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
        {
            standingOnTile.topNeighbour.BlockingTilePiece.TakeDamage(stats.AttackPower);
        }

        if (standingOnTile.rightNeighbour != null && standingOnTile.rightNeighbour.BlockingTilePiece != null && LayerUtilities.IsObjectInLayer(standingOnTile.rightNeighbour.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
        {
            standingOnTile.rightNeighbour.BlockingTilePiece.TakeDamage(stats.AttackPower);
        }

        if (standingOnTile.leftNeighbour != null && standingOnTile.leftNeighbour.BlockingTilePiece != null && LayerUtilities.IsObjectInLayer(standingOnTile.leftNeighbour.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
        {
            standingOnTile.leftNeighbour.BlockingTilePiece.TakeDamage(stats.AttackPower);
        }

        if (standingOnTile.botNeighbour != null && standingOnTile.botNeighbour.BlockingTilePiece != null && LayerUtilities.IsObjectInLayer(standingOnTile.botNeighbour.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
        {
            standingOnTile.botNeighbour.BlockingTilePiece.TakeDamage(stats.AttackPower);
        }
    }
}
