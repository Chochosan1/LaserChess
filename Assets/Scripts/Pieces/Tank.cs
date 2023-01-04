using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Tank : Piece
{
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private Vector3 projectileSpawnOffset = new Vector3(0f, 75f, 0f);

    //orthogonal paths
    private List<GridTile> currentTopTileRoute;
    private List<GridTile> currentBotTileRoute;
    private List<GridTile> currentRightTileRoute;
    private List<GridTile> currentLeftTileRoute;

    //diagonal paths
    private List<GridTile> currentTopRightTileRoute;
    private List<GridTile> currentTopLeftTileRoute;
    private List<GridTile> currentBotRightTileRoute;
    private List<GridTile> currentBotLeftTileRoute;

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
        currentTopTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.Top);
        currentBotTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.Bot);
        currentRightTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.Right);
        currentLeftTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.Left);
        currentTopRightTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.TopRight);
        currentTopLeftTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.TopLeft);
        currentBotLeftTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.BotLeft);
        currentBotRightTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.BotRight);

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

        foreach (GridTile gridTile in currentTopRightTileRoute)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentTopLeftTileRoute)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentBotRightTileRoute)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentBotLeftTileRoute)
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

        foreach (GridTile gridTile in currentTopRightTileRoute)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentTopLeftTileRoute)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentBotRightTileRoute)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentBotLeftTileRoute)
        {
            gridTile.DeactivateTile();
        }
    }

    protected override void Attack()
    {
        isEnemyFoundDuringProbing = false;
        currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.Top, true);

        if (!isEnemyFoundDuringProbing)
        {
            foreach (GridTile tile in currentAttackPath)
            {
                //find an occupied tile and check if the piece on top of it is in the must-damage layer
                if (tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                    Debug.Log($"WILL SHOOT ON THE TOP TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    break;
                }
            }
        }

        //don't continue enemy probing if an enemy has already been found
        if (!isEnemyFoundDuringProbing)
        {
            currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.Right, true);

            foreach (GridTile tile in currentAttackPath)
            {
                if (tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                    Debug.Log($"WILL SHOOT ON THE RIGHT TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                    break;
                }
            }
        }


        if (!isEnemyFoundDuringProbing)
        {
            currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.Left, true);

            foreach (GridTile tile in currentAttackPath)
            {
                if (tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                    Debug.Log($"WILL SHOOT ON THE LEFT TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                    break;
                }
            }
        }

        if (!isEnemyFoundDuringProbing)
        {
            currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, MapController.Directions.Bot, true);

            foreach (GridTile tile in currentAttackPath)
            {
                if (tile.IsBlocked && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;
                    Debug.Log($"WILL SHOOT ON THE BOT TO DAMAGE PIECE {tile.BlockingTilePiece.gameObject}");
                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);
                    break;
                }
            }
        }
    }
}
