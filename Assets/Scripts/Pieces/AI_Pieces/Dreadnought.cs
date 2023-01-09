using LaserChess.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dreadnought : Piece, IAutoRunnableAI
{
    [Header("AI Priority Group")]
    [Tooltip("The priority group this Dreadnought should be part of. Order of execution is: One -> Two -> Three.")]
    [SerializeField] private GameStateManager.AI_TurnPriority turnPriorityGroup = GameStateManager.AI_TurnPriority.Two;

    //attack
    private List<GridTile> currentAttackPath; //reuse the same list to probe for different attack paths

    //movement
    private GridTile currentGridTileToMoveTo;
    private float step;
    private bool isActivatedAndMustPlay = false; //should the AI behaviour logic execute?

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
                StandingOnTile = currentGridTileToMoveTo;

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
        StandingOnTile.MarkTileAsFree();

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        currentGridTileToMoveTo.MarkTileAsBlocked(this); //mark it as blocked immediately so that clicking on another unit won't show that tile as free while another unit is traveling to it
    }

    protected override void Die()
    {
        GameStateManager.Instance.RemoveDestroyedAIUnit(this);
        base.Die();
    }


    //dreadnoughts attack all adjacent enemies simulatenously (including the diagonally adjacent)
    protected override void Attack()
    {
        //jumpship attacks simulatenously in all adjacent tiles (all directions 1 tile away)
        for (int currentEnumIndex = 0; currentEnumIndex < 9; currentEnumIndex++)
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

        Vector3Int closestPlayerPiecePosition = GetClosestEnemy(GameStateManager.Instance.PlayerPieces).StandingOnTile.gridLocation;
        Vector3Int thisPiecePosition = StandingOnTile.gridLocation;

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
            if (StandingOnTile.RightNeighbour != null && !StandingOnTile.RightNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.RightNeighbour);
            else if (StandingOnTile.BotRightNeighbour != null && !StandingOnTile.BotRightNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.BotRightNeighbour);
            else if (StandingOnTile.TopRightNeighbour != null && !StandingOnTile.TopRightNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.TopRightNeighbour);
        }

        //enemy is to the bot side (try bot, botleft & botright)
        if (closestPlayerPiecePosition.x == thisPiecePosition.x && closestPlayerPiecePosition.z < thisPiecePosition.z)
        {
            if (StandingOnTile.BotNeighbour != null && !StandingOnTile.BotNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.BotNeighbour);
            else if (StandingOnTile.BotRightNeighbour != null && !StandingOnTile.BotRightNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.BotRightNeighbour);
            else if (StandingOnTile.BotLeftNeighbour != null && !StandingOnTile.BotLeftNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.BotLeftNeighbour);
        }

        //enemy is to the top side (try top, topright & topleft)
        if (closestPlayerPiecePosition.x == thisPiecePosition.x && closestPlayerPiecePosition.z > thisPiecePosition.z)
        {
            if (StandingOnTile.TopNeighbour != null && !StandingOnTile.TopNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.TopNeighbour);
            else if (StandingOnTile.TopRightNeighbour != null && !StandingOnTile.TopRightNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.TopRightNeighbour);
            else if (StandingOnTile.TopLeftNeighbour != null && !StandingOnTile.TopLeftNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.TopLeftNeighbour);
        }

        //enemy is to the left (try left, botleft & topleft)
        if (closestPlayerPiecePosition.x < thisPiecePosition.x && closestPlayerPiecePosition.z == thisPiecePosition.z)
        {
            if (StandingOnTile.LeftNeighbour != null && !StandingOnTile.LeftNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.LeftNeighbour);
            else if (StandingOnTile.TopLeftNeighbour != null && !StandingOnTile.TopLeftNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.TopLeftNeighbour);
            else if (StandingOnTile.BotLeftNeighbour != null && !StandingOnTile.BotLeftNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.BotLeftNeighbour);
        }

        //enemy is to the top right (try topright, right & top)
        if (closestPlayerPiecePosition.x > thisPiecePosition.x && closestPlayerPiecePosition.z > thisPiecePosition.z)
        {
            if (StandingOnTile.TopRightNeighbour != null && !StandingOnTile.TopRightNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.TopRightNeighbour);
            else if (StandingOnTile.RightNeighbour != null && !StandingOnTile.RightNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.RightNeighbour);
            else if (StandingOnTile.TopNeighbour != null && !StandingOnTile.TopNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.TopNeighbour);
        }

        //enemy is to the top left
        if (closestPlayerPiecePosition.x < thisPiecePosition.x && closestPlayerPiecePosition.z > thisPiecePosition.z)
        {
            if (StandingOnTile.TopLeftNeighbour != null && !StandingOnTile.TopLeftNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.TopLeftNeighbour);
            else if (StandingOnTile.LeftNeighbour != null && !StandingOnTile.LeftNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.LeftNeighbour);
            else if (StandingOnTile.TopNeighbour != null && !StandingOnTile.TopNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.TopNeighbour);
        }

        //enemy is to the bottom right somewhere (try to move botRight first, if not possible try right & then try bot)
        if (closestPlayerPiecePosition.x > thisPiecePosition.x && closestPlayerPiecePosition.z < thisPiecePosition.z)
        {
            if (StandingOnTile.BotRightNeighbour != null && !StandingOnTile.BotRightNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.BotRightNeighbour);
            else if (StandingOnTile.RightNeighbour != null && !StandingOnTile.RightNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.RightNeighbour);
            else if (StandingOnTile.BotNeighbour != null && !StandingOnTile.BotNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.BotNeighbour);
        }

        //enemy is to the bot left
        if (closestPlayerPiecePosition.x < thisPiecePosition.x && closestPlayerPiecePosition.z < thisPiecePosition.z)
        {
            if (StandingOnTile.BotLeftNeighbour != null && !StandingOnTile.BotLeftNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.BotLeftNeighbour);
            else if (StandingOnTile.LeftNeighbour != null && !StandingOnTile.LeftNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.LeftNeighbour);
            else if (StandingOnTile.BotNeighbour != null && !StandingOnTile.BotNeighbour.IsBlocked)
                OnMoveCommand(StandingOnTile.BotNeighbour);
        }
    }

    public bool IsAutoRunDone() => hasPlayedItsTurn;
    public GameObject GetGameObject() => this.gameObject;
    public GameStateManager.AI_TurnPriority GetAITurnPriority() => turnPriorityGroup;
}
