using LaserChess.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dreadnought : Piece, IAutoRunnableAI
{
    private GridTile currentGridTileToMoveTo;
    private float step;
    private bool isActivatedAndMustPlay = false; //should the AI behaviour logic execute?
    private bool hasPlayedItsTurn = false; //has the AI behaviour logic executed to the end?

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (!isActivatedAndMustPlay)
            return;

        if (currentGridTileToMoveTo != null)
        {
            step = movementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentGridTileToMoveTo.transform.position.x, transform.position.y, currentGridTileToMoveTo.transform.position.z), step);

            //when the targeted tile has been reached
            if (Vector3.Distance(transform.position, new Vector3(currentGridTileToMoveTo.transform.position.x, transform.position.y, currentGridTileToMoveTo.transform.position.z)) < 0.01f)
            {
                standingOnTile = currentGridTileToMoveTo;

                Attack();

                isActivatedAndMustPlay = false;
                hasPlayedItsTurn = true;

                Debug.Log("DREADNOUGHT move and attacked");
            }
        }
        else
        {
            Attack();

            isActivatedAndMustPlay = false;
            hasPlayedItsTurn = true;

            Debug.Log("DREADNOUGHT couldn't move but attacked");
        }
    }

    public override void OnMoveCommand(GridTile selectedGridTileToMoveTo)
    {
        standingOnTile.MarkTileAsFree();

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        currentGridTileToMoveTo.MarkTileAsBlocked(this); //mark it as blocked immediately so that clicking on another unit won't show that tile as free while another unit is traveling to it
    }

    public override void OnSelectedPiece() { }


    public override void OnDeselectedPiece() { }


    //dreadnoughts attack all adjacent enemies simulatenously (including the diagonally adjacent)
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

        if (standingOnTile.topRightNeighbour != null && standingOnTile.topRightNeighbour.BlockingTilePiece != null && LayerUtilities.IsObjectInLayer(standingOnTile.topRightNeighbour.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
        {
            standingOnTile.topRightNeighbour.BlockingTilePiece.TakeDamage(stats.AttackPower);
        }

        if (standingOnTile.topLeftNeighbour != null && standingOnTile.topLeftNeighbour.BlockingTilePiece != null && LayerUtilities.IsObjectInLayer(standingOnTile.topLeftNeighbour.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
        {
            standingOnTile.topLeftNeighbour.BlockingTilePiece.TakeDamage(stats.AttackPower);
        }

        if (standingOnTile.botRightNeighbour != null && standingOnTile.botRightNeighbour.BlockingTilePiece != null && LayerUtilities.IsObjectInLayer(standingOnTile.botRightNeighbour.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
        {
            standingOnTile.botRightNeighbour.BlockingTilePiece.TakeDamage(stats.AttackPower);
        }

        if (standingOnTile.botLeftNeighbour != null && standingOnTile.botLeftNeighbour.BlockingTilePiece != null && LayerUtilities.IsObjectInLayer(standingOnTile.botLeftNeighbour.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
        {
            standingOnTile.botLeftNeighbour.BlockingTilePiece.TakeDamage(stats.AttackPower);
        }
    }

    //returns the closest enemy based on its absolute distance from the dreadnought
    private Piece GetClosestEnemy(List<Piece> enemies)
    {
        Piece closestPiece = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Piece piece in enemies)
        {
            float dist = Vector3.Distance(piece.transform.position, currentPos);
            if (dist < minDist)
            {
                closestPiece = piece;
                minDist = dist;
            }
        }

        Debug.Log($"DREADNOUGHT will go to {closestPiece.name}");
        return closestPiece;
    }

    public void AutoRunBehaviour()
    {
        isActivatedAndMustPlay = true;
        hasPlayedItsTurn = false;
        currentGridTileToMoveTo = null;

        Vector3Int closestPlayerPiecePosition = GetClosestEnemy(GameStateManager.Instance.PlayerPieces).standingOnTile.gridLocation;
        Vector3Int thisPiecePosition = standingOnTile.gridLocation;

        //if the enemy is on an adjacent tile (both orthogonally & diagonally) then don't try to move
        if((Mathf.Abs(closestPlayerPiecePosition.x - thisPiecePosition.x) <= 1) && Mathf.Abs(closestPlayerPiecePosition.z - thisPiecePosition.z) <= 1)
        {
            Debug.Log($"DREADNOUGHT will NOT move as its target is adjacent");
            return;
        }
         
        //PATHFINDING NOTE!! Conditions below take into consideration which quadrant relative to the dreadnought the enemy is in. Then the dreadnought will try going closer
        //by using the tiles closest to that quadrant (e.g topRight will use topRight tile, top tile & right tile, giving priority to topRight as its a shorter path).
        //In extremely rare cases, if all three tiles are taken the dreadnought will not try to pathfind around all obstacles, it will instead wait until one of them gets free.
        //This could be avoided (if needed) by adding more than 3 tile options or implementing the AStar algorithm (which I find unnecessary in the current use-case).


        //enemy is to the right (try right, botright & topright)
        if (closestPlayerPiecePosition.x > thisPiecePosition.x && closestPlayerPiecePosition.z == thisPiecePosition.z)
        {
            if (standingOnTile.rightNeighbour != null && !standingOnTile.rightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.rightNeighbour);
            else if (standingOnTile.botRightNeighbour != null && !standingOnTile.botRightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.botRightNeighbour);
            else if (standingOnTile.topRightNeighbour != null && !standingOnTile.topRightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.topRightNeighbour);
        }

        //enemy is to the bot side (try bot, botleft & botright)
        if (closestPlayerPiecePosition.x == thisPiecePosition.x && closestPlayerPiecePosition.z < thisPiecePosition.z)
        {
            if (standingOnTile.botNeighbour != null && !standingOnTile.botNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.botNeighbour);
            else if (standingOnTile.botRightNeighbour != null && !standingOnTile.botRightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.botRightNeighbour);
            else if (standingOnTile.botLeftNeighbour != null && !standingOnTile.botLeftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.botLeftNeighbour);
        }

        //enemy is to the top side (try top, topright & topleft)
        if (closestPlayerPiecePosition.x == thisPiecePosition.x && closestPlayerPiecePosition.z > thisPiecePosition.z)
        {
            if (standingOnTile.topNeighbour != null && !standingOnTile.topNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.topNeighbour);
            else if (standingOnTile.topRightNeighbour != null && !standingOnTile.topRightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.topRightNeighbour);
            else if (standingOnTile.topLeftNeighbour != null && !standingOnTile.topLeftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.topLeftNeighbour);
        }

        //enemy is to the left (try left, botleft & topleft)
        if (closestPlayerPiecePosition.x < thisPiecePosition.x && closestPlayerPiecePosition.z == thisPiecePosition.z)
        {
            if (standingOnTile.leftNeighbour != null && !standingOnTile.leftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.leftNeighbour);
            else if (standingOnTile.topLeftNeighbour != null && !standingOnTile.topLeftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.topLeftNeighbour);
            else if (standingOnTile.botLeftNeighbour != null && !standingOnTile.botLeftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.botLeftNeighbour);
        }

        //enemy is to the top right (try topright, right & top)
        if (closestPlayerPiecePosition.x > thisPiecePosition.x && closestPlayerPiecePosition.z > thisPiecePosition.z)
        {
            if (standingOnTile.topRightNeighbour != null && !standingOnTile.topRightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.topRightNeighbour);
            else if (standingOnTile.rightNeighbour != null && !standingOnTile.rightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.rightNeighbour);
            else if (standingOnTile.topNeighbour != null && !standingOnTile.topNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.topNeighbour);
        }

        //enemy is to the top left
        if (closestPlayerPiecePosition.x < thisPiecePosition.x && closestPlayerPiecePosition.z > thisPiecePosition.z)
        {
            if (standingOnTile.topLeftNeighbour != null && !standingOnTile.topLeftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.topLeftNeighbour);
            else if (standingOnTile.leftNeighbour != null && !standingOnTile.leftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.leftNeighbour);
            else if (standingOnTile.topNeighbour != null && !standingOnTile.topNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.topNeighbour);
        }

        //enemy is to the bottom right somewhere (try to move botRight first, if not possible try right & then try bot)
        if (closestPlayerPiecePosition.x > thisPiecePosition.x && closestPlayerPiecePosition.z < thisPiecePosition.z)
        {
            if (standingOnTile.botRightNeighbour != null && !standingOnTile.botRightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.botRightNeighbour);
            else if (standingOnTile.rightNeighbour != null && !standingOnTile.rightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.rightNeighbour);
            else if (standingOnTile.botNeighbour != null && !standingOnTile.botNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.botNeighbour);
        }

        //enemy is to the bot left
        if (closestPlayerPiecePosition.x < thisPiecePosition.x && closestPlayerPiecePosition.z < thisPiecePosition.z)
        {
            if (standingOnTile.botLeftNeighbour != null && !standingOnTile.botLeftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.botLeftNeighbour);
            else if (standingOnTile.leftNeighbour != null && !standingOnTile.leftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.leftNeighbour);
            else if (standingOnTile.botNeighbour != null && !standingOnTile.botNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.botNeighbour);
        }
    }

    public bool IsAutoRunDone() => hasPlayedItsTurn;
}
