using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Piece, IAutoRunnableAI
{
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private Vector3 projectileSpawnOffset = new Vector3(0f, 1f, 0f);

    //attack
    private List<GridTile> currentAttackPath; //reuse the same list to probe for different attack paths (e.g the 4 diagonals until an enemy is found)
    private bool isEnemyFoundDuringProbing = false;

    //movement
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

        if(currentGridTileToMoveTo != null)
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

                Debug.Log("DRONE move and attacked");
            }
        }
        else
        {
            Attack();

            isActivatedAndMustPlay = false;
            hasPlayedItsTurn = true;

            Debug.Log("DRONE couldn't move but attacked");
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


    //grunts attack diagonally in any range; probes all diagonal directions until it finds an enemy in one of them (will actually damage only in one diagonal)
    protected override void Attack()
    {
        isEnemyFoundDuringProbing = false;
        currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.TopRight, true);

        if (!isEnemyFoundDuringProbing)
        {
            foreach (GridTile tile in currentAttackPath)
            {
                //find an occupied tile and check if the piece on top of it is in the must-damage layer
                if (tile.BlockingTilePiece != null && tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                //    Debug.Log($"WILL SHOOT ON THE TOP RIGHT DIAGONAL TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    break;
                }
            }
        }

        //don't continue enemy probing if an enemy has already been found
        if (!isEnemyFoundDuringProbing)
        {
            currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.TopLeft, true);

            foreach (GridTile tile in currentAttackPath)
            {
                if (tile.BlockingTilePiece != null && tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                //    Debug.Log($"WILL SHOOT ON THE TOP LEFT DIAGONAL TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                    break;
                }
            }
        }


        if (!isEnemyFoundDuringProbing)
        {
            currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.BotLeft, true);

            foreach (GridTile tile in currentAttackPath)
            {
                if (tile.BlockingTilePiece != null && tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                //    Debug.Log($"WILL SHOOT ON THE BOT LEFT DIAGONAL TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                    break;
                }
            }
        }

        if (!isEnemyFoundDuringProbing)
        {
            currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.BotRight, true);

            foreach (GridTile tile in currentAttackPath)
            {
                if (tile.BlockingTilePiece != null && tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                //    Debug.Log($"WILL SHOOT ON THE BOT RIGHT DIAGONAL TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                    break;
                }
            }
        }
    }

    public void AutoRunBehaviour()
    {
        isActivatedAndMustPlay = true;
        hasPlayedItsTurn = false;

        if (standingOnTile.botNeighbour != null && !standingOnTile.botNeighbour.IsBlocked)
            OnMoveCommand(standingOnTile.botNeighbour);
        else
            currentGridTileToMoveTo = null;
    }

    public bool IsAutoRunDone() => hasPlayedItsTurn;
}
