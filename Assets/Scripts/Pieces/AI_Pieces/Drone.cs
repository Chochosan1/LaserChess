using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Piece, IAutoRunnableAI
{
    [Header("AI Priority Group")]
    [SerializeField] private GameStateManager.AI_TurnPriority turnPriorityGroup = GameStateManager.AI_TurnPriority.One;

    [Header("Shooting")]
    [SerializeField] private ProjectileController projectilePrefab;
    [SerializeField] private Vector3 projectileSpawnOffset = new Vector3(0f, 1f, 0f);

    //attack
    private List<GridTile> currentAttackPath; //reuse the same list to probe for different attack paths (e.g the 4 diagonals until an enemy is found)
    private bool isEnemyFoundDuringProbing = false;

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


    //drones attack diagonally in any range; probes all diagonal directions until it finds an enemy in one of them (will actually damage only in one diagonal)
    protected override void Attack()
    {
        isEnemyFoundDuringProbing = false;

        //the last 4 elements in the enum are the diagonal directions in which the drone shoots
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

    protected override void Die()
    {
        GameStateManager.Instance.RemoveDestroyedAIUnit(this);
        base.Die();
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

    public GameObject GetGameObject() => this.gameObject;

    public GameStateManager.AI_TurnPriority GetAITurnPriority() => turnPriorityGroup;
}
