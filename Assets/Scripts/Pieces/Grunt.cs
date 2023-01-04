using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : Piece
{
    //list all possible paths here
    private List<GridTile> currentTopTileRoute;
    private List<GridTile> currentBotTileRoute;
    private List<GridTile> currentRightTileRoute;
    private List<GridTile> currentLeftTileRoute;

    private List<GridTile> currentTopRightTileRoute;
    private List<GridTile> currentKnightTopRight1Route;
    private List<GridTile> currentKnightTopRight2Route;

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
            standingOnTile.isBlocked = true;
            isMoving = false;
        }
    }

    public override void OnMoveCommand(GridTile selectedGridTileToMoveTo)
    {
        base.OnMoveCommand(selectedGridTileToMoveTo);

        standingOnTile.isBlocked = false;

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        isMoving = true;
    }

    public override void OnSelectedPiece()
    {
        base.OnSelectedPiece();

        currentTopTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 1, MapController.Directions.Top);
        currentBotTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 1, MapController.Directions.Bot);
        currentRightTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 1, MapController.Directions.Right);
        currentLeftTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 1, MapController.Directions.Left);
        //currentTopRightTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 5, MapController.Directions.TopRight);
        //currentKnightTopRight1Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightTopRight1);
        //currentKnightTopRight2Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightTopRight2);

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

        //foreach (GridTile gridTile in currentTopRightTileRoute)
        //{
        //    gridTile.ActivateTile();
        //}

        //foreach (GridTile gridTile in currentKnightTopRight1Route)
        //{
        //    gridTile.ActivateTile();
        //}

        //foreach (GridTile gridTile in currentKnightTopRight2Route)
        //{
        //    gridTile.ActivateTile();
        //}
    }

    public override void OnDeselectedPiece()
    {
        base.OnDeselectedPiece();

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

        //foreach (GridTile gridTile in currentTopRightTileRoute)
        //{
        //    gridTile.DeactivateTile();
        //}

        //foreach (GridTile gridTile in currentKnightTopRight1Route)
        //{
        //    gridTile.DeactivateTile();
        //}

        //foreach (GridTile gridTile in currentKnightTopRight2Route)
        //{
        //    gridTile.DeactivateTile();
        //}
    }
}
