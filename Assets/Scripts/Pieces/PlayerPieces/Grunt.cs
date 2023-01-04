using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Grunt : Piece
{
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private Vector3 projectileSpawnOffset = new Vector3(0f, 1f, 0f);

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

    //grunts attack diagonally in any range; probes all diagonal directions until it finds an enemy in one of them (will actually damage only in one diagonal)
    protected override void Attack()
    {
        isEnemyFoundDuringProbing = false;
        currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.TopRight, true);

        if(!isEnemyFoundDuringProbing)
        {
            foreach (GridTile tile in currentAttackPath)
            {
                //find an occupied tile and check if the piece on top of it is in the must-damage layer
                if (tile.BlockingTilePiece != null && tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                    Debug.Log($"WILL SHOOT ON THE TOP RIGHT DIAGONAL TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
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
                    Debug.Log($"WILL SHOOT ON THE TOP LEFT DIAGONAL TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
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
                    Debug.Log($"WILL SHOOT ON THE BOT LEFT DIAGONAL TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
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
                    Debug.Log($"WILL SHOOT ON THE BOT RIGHT DIAGONAL TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                    break;
                }
            }
        }
    }
}
