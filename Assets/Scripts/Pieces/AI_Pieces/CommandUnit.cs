using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandUnit : Piece, IAutoRunnableAI
{
    [Header("AI Priority Group")]
    [Tooltip("The priority group this Command Unit should be part of. Order of execution is: One -> Two -> Three.")]
    [SerializeField] private GameStateManager.AI_TurnPriority turnPriorityGroup = GameStateManager.AI_TurnPriority.Three;

    //movement
    private GridTile currentGridTileToMoveTo;
    private float step;
    private bool isActivatedAndMustPlay = false; //should the AI behaviour logic execute?
    private List<GridTile> checkForEnemiesToAvoidPath = new List<GridTile>();

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

                isActivatedAndMustPlay = false;
                hasPlayedItsTurn = true;

                Debug.Log("COMMAND UNIT move");
            }
        }
        else
        {
            isActivatedAndMustPlay = false;
            hasPlayedItsTurn = true;

            Debug.Log("COMMAND UNIT couldn't move");
        }
    }

    public override void OnMoveCommand(GridTile selectedGridTileToMoveTo)
    {
        StandingOnTile.MarkTileAsFree();

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        currentGridTileToMoveTo.MarkTileAsBlocked(this); //mark it as blocked immediately so that clicking on another unit won't show that tile as free while another unit is traveling to it
    }

    protected override void Attack() { }

    protected override void Die()
    {
        GameStateManager.Instance.RemoveDestroyedAIUnit(this);
        base.Die();
    }

    public void AutoRunBehaviour()
    {
        isActivatedAndMustPlay = true;
        hasPlayedItsTurn = false;

        currentGridTileToMoveTo = null;
        bool threatFound = false;

        //Prio 1: check for tanks on the bottom orgthogonal path and move left/right (avoids staying in the direct line of fire)
        //(top is not possible as command units stay always on last top row and they can only move left/right so left/right/top tank checking is useless)
        checkForEnemiesToAvoidPath = MapController.Instance.GetPossibleRouteFromTile(StandingOnTile, 10, MapController.Directions.Bot, true);
        foreach (GridTile tile in checkForEnemiesToAvoidPath)
        {
            if(tile.BlockingTilePiece != null && tile.BlockingTilePiece.GetComponent<Tank>() != null)
            {
                OnMoveCommand(GetRandomLeftOrRightTile());
                return;
            }
        }

        //Prio 2: Try moving left/right if there's no tank in direct line of sight on the new tile. Also, there should be no grunts on the CURRENT diagonals (because if they are 
        //in the current diagonals they would need at least 2 rounds to attack, whereas if the command unit moves it may reduce that number to only 1 round). This makes it
        //essentially impossible to win with only 1 grunt vs the command unit
        GridTile currentNextTile = StandingOnTile.LeftNeighbour;
        for(int i = 0; i < 2; i++)
        {
            threatFound = false;
            if (i == 1)
                currentNextTile = StandingOnTile.RightNeighbour;

            if (currentNextTile != null)
            {
                //if there's a tank in direct line of sight on the new tile then just abandon
                checkForEnemiesToAvoidPath = MapController.Instance.GetPossibleRouteFromTile(currentNextTile, 10, MapController.Directions.Bot, true);
                foreach (GridTile tile in checkForEnemiesToAvoidPath)
                {
                    if (tile.BlockingTilePiece != null && tile.BlockingTilePiece.GetComponent<Tank>() != null)
                    {
                        threatFound = true;
                        Debug.Log("Tank threat found!");
                        break;
                    }
                }

                //if there's a grunt on the current left diagonal then just abandon (else it can just move orthogonally one tile next round and attack directly, whereas
                //not moving will have the grunt lose at least 1 round not attacking)
                checkForEnemiesToAvoidPath = MapController.Instance.GetPossibleRouteFromTile(StandingOnTile, 10, MapController.Directions.BotLeft, true);
                foreach (GridTile tile in checkForEnemiesToAvoidPath)
                {
                    if (tile.BlockingTilePiece != null && tile.BlockingTilePiece.GetComponent<Grunt>() != null)
                    {
                        threatFound = true;
                        Debug.Log("Grunt botleft threat found!");
                        break;
                    }
                }

                //if there's a grunt on the current right diagonal then just abandon (else it can just move orthogonally one tile next round and attack directly, whereas
                //not moving will have the grunt lose at least 1 round not attacking)
                checkForEnemiesToAvoidPath = MapController.Instance.GetPossibleRouteFromTile(StandingOnTile, 10, MapController.Directions.BotRight, true);
                foreach (GridTile tile in checkForEnemiesToAvoidPath)
                {
                    if (tile.BlockingTilePiece != null && tile.BlockingTilePiece.GetComponent<Grunt>() != null)
                    {
                        threatFound = true;
                        Debug.Log("Grunt botright threat found!");
                        break;
                    }
                }

                if (!threatFound)
                {
                    OnMoveCommand(currentNextTile);
                    return;
                }
            }
        }
    }

    private GridTile GetRandomLeftOrRightTile()
    {
        GridTile gridTileChosen = null;
        int direction = Random.Range(0, 2);
        //pick randomly to move left or right 
        if (direction == 0)
        {
            //prioritize left move; if not available go right
            if (StandingOnTile.LeftNeighbour != null && !StandingOnTile.LeftNeighbour.IsBlocked)
                gridTileChosen = StandingOnTile.LeftNeighbour;      
            else if (StandingOnTile.RightNeighbour != null && !StandingOnTile.RightNeighbour.IsBlocked)
                gridTileChosen = StandingOnTile.RightNeighbour;
        }
        else if (direction == 1)
        {
            //prioritize right move; if not available go left
            if (StandingOnTile.RightNeighbour != null && !StandingOnTile.RightNeighbour.IsBlocked)
                gridTileChosen = StandingOnTile.RightNeighbour;    
            else if (StandingOnTile.LeftNeighbour != null && !StandingOnTile.LeftNeighbour.IsBlocked)
                gridTileChosen = StandingOnTile.LeftNeighbour;      
        }

        return gridTileChosen;
    }

    public bool IsAutoRunDone() => hasPlayedItsTurn;
    public GameObject GetGameObject() => this.gameObject;
    public GameStateManager.AI_TurnPriority GetAITurnPriority() => turnPriorityGroup;
}
