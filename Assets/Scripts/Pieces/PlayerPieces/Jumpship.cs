using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserChess.Utilities;
public sealed class Jumpship : Piece
{
    //attack
    private List<GridTile> currentAttackPath; //reuse the same list to probe for different attack paths

    //movement
    private List<GridTile> allPathsUsedTiles = new List<GridTile>();
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
            StandingOnTile = currentGridTileToMoveTo;
            isMoving = false;

            Attack();
        }
    }

    public override void OnMoveCommand(GridTile selectedGridTileToMoveTo)
    {
        StandingOnTile.MarkTileAsFree();

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        currentGridTileToMoveTo.MarkTileAsBlocked(this); //mark it as blocked immediately so that clicking on another unit won't show that tile as free while another unit is traveling to it
        isMoving = true;
        hasPlayedItsTurn = true;
    }

    public override void OnSelectedPiece()
    {
        if (hasPlayedItsTurn)
            return;

        allPathsUsedTiles = new List<GridTile>();
        List<GridTile> currentMovePath = new List<GridTile>();

        //finds the end point for each knight pattern and activates the tile (ready to be selected by the player); 
        foreach (int currKnightPattern in System.Enum.GetValues(typeof(MapController.KnightPattern)))
        {
            currentMovePath = MapController.Instance.GetPossibleKnightRouteFromTile(StandingOnTile, (MapController.KnightPattern)currKnightPattern);

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
        //the first 4 elements in the enum are the orthogonal directions in which the jumpship attacks simulatenously
        for (int currentEnumIndex = 0; currentEnumIndex < 4; currentEnumIndex++)
        {
            //get the current attack path (based on the current enum direction)
            currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(StandingOnTile, 1, (MapController.Directions)currentEnumIndex, true);

            //probe the attack path to find if an enemy is there to attack it
            foreach (GridTile tile in currentAttackPath)
            {
                if (tile.BlockingTilePiece != null && LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, DamagePiecesOnThisLayer))
                {
                    Debug.Log($"I damaged {tile.BlockingTilePiece.name}");
                    tile.BlockingTilePiece.TakeDamage(stats.AttackPower);
                }
            }
        }
    }
}
