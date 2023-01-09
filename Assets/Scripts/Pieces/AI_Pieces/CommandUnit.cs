using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandUnit : Piece, IAutoRunnableAI
{
    [Header("AI Priority Group")]
    [SerializeField] private GameStateManager.AI_TurnPriority turnPriorityGroup = GameStateManager.AI_TurnPriority.Three;

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
                standingOnTile = currentGridTileToMoveTo;

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
        standingOnTile.MarkTileAsFree();

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        currentGridTileToMoveTo.MarkTileAsBlocked(this); //mark it as blocked immediately so that clicking on another unit won't show that tile as free while another unit is traveling to it
    }

    public override void OnSelectedPiece() { }

    public override void OnDeselectedPiece() { }

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

        int direction = Random.Range(0, 2);

        //pick randomly to move left or right 
        if (direction == 0)
        {
            //prioritize left move; if not available go right
            if (standingOnTile.leftNeighbour != null && !standingOnTile.leftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.leftNeighbour);
            else if (standingOnTile.rightNeighbour != null && !standingOnTile.rightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.rightNeighbour);

        }
        else if (direction == 1)
        {
            if (standingOnTile.rightNeighbour != null && !standingOnTile.rightNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.rightNeighbour);
            else if (standingOnTile.leftNeighbour != null && !standingOnTile.leftNeighbour.IsBlocked)
                OnMoveCommand(standingOnTile.leftNeighbour);
        }
    }

    public bool IsAutoRunDone() => hasPlayedItsTurn;
    public GameObject GetGameObject() => this.gameObject;
    public GameStateManager.AI_TurnPriority GetAITurnPriority() => turnPriorityGroup;
}
