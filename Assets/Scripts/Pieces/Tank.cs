using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Piece
{
    //list all possible paths here
    private List<GridTile> currentTopTileRoute;
    private List<GridTile> currentBotTileRoute;
    private List<GridTile> currentRightTileRoute;
    private List<GridTile> currentLeftTileRoute;

    private List<GridTile> currentTopRightTileRoute;
    private List<GridTile> currentTopLeftTileRoute;
    private List<GridTile> currentBotRightTileRoute;
    private List<GridTile> currentBotLeftTileRoute;


    private GridTile currentGridTileToMoveTo;
    private bool isMoving;
    private float step;

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
        //    standingOnTile.isBlocked = true;
            isMoving = false;
        }
    }

    public override void OnMoveCommand(GridTile selectedGridTileToMoveTo)
    {
        standingOnTile.isBlocked = false;

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        currentGridTileToMoveTo.isBlocked = true; //mark it as blocked immediately so that clicking on another unit won't show that tile as free while another unit is traveling to it
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
}
