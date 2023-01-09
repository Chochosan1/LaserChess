using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Grunt : Piece
{
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private Vector3 projectileSpawnOffset = new Vector3(0f, 1f, 0f);

    //attack
    private List<GridTile> currentAttackPath; //reuse the same list to probe for different attack paths
    private bool isEnemyFoundDuringProbing = false;

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
        hasPlayedItsTurn = true;
    }

    public override void OnSelectedPiece()
    {
        if (hasPlayedItsTurn)
            return;

        allPathsUsedTiles = new List<GridTile>();
        List<GridTile> currentMovePath = new List<GridTile>();

        //the first 4 elements in the enum are the orthogonal directions in which the grunt moves
        for (int currentEnumIndex = 0; currentEnumIndex < 4; currentEnumIndex++)
        {
            //get the current attack path (based on the current enum direction)
            currentMovePath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 1, (MapController.Directions)currentEnumIndex);

            //probe the attack path to find if an enemy is there to attack it
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

    //grunts attack diagonally in any range; probes all diagonal directions until it finds an enemy in one of them (will actually damage only in one diagonal)
    protected override void Attack()
    {
        isEnemyFoundDuringProbing = false;

        //the last 4 elements in the enum are the diagonal directions in which the grunt shoots
        for (int currentEnumIndex = 4; currentEnumIndex < 8; currentEnumIndex++)
        {
            //get the current attack path (based on the current enum direction)
            currentAttackPath = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 10, (MapController.Directions)currentEnumIndex, true);

            //probe the attack path to find if an enemy is there to attack it
            foreach (GridTile tile in currentAttackPath)
            {
                if (tile.BlockingTilePiece != null && LaserChess.Utilities.LayerUtilities.IsObjectInLayer(tile.BlockingTilePiece.gameObject, damagePiecesOnThisLayer))
                {
                    isEnemyFoundDuringProbing = true;

                    ProjectileController projectileCopy = Instantiate(projectilePrefab, transform.position + projectileSpawnOffset, Quaternion.identity);
                    projectileCopy.SetupProjectile(this, tile.BlockingTilePiece);

                    break;
                }
            }

            if (isEnemyFoundDuringProbing)
                break;
        }
    }
}
