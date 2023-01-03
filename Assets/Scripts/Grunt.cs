using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : Piece
{
    private List<GridTile> currentTopTileRoute;
    private List<GridTile> currentBotTileRoute;
    private List<GridTile> currentRightTileRoute;
    private List<GridTile> currentLeftTileRoute;


    private List<GridTile> currentTopRightTileRoute;

    //  private List<GridTile> currentPathToMoveThrough;
    private GridTile currentGridTileToMoveTo;
    private bool isMoving;

    private void Update()
    {
        if (!isMoving)
            return;

        var step = movementSpeed * Time.deltaTime;
     //   transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentPathToMoveThrough[currentPathToMoveThrough.Count - 1].transform.position.x, transform.position.y, currentPathToMoveThrough[currentPathToMoveThrough.Count - 1].transform.position.z), step);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentGridTileToMoveTo.transform.position.x, transform.position.y, currentGridTileToMoveTo.transform.position.z), step);

        if (Vector3.Distance(transform.position, new Vector3(currentGridTileToMoveTo.transform.position.x, transform.position.y, currentGridTileToMoveTo.transform.position.z)) < 0.01f)
        {
            standingOnTile = currentGridTileToMoveTo;
            isMoving = false;
        }
    }

    public override void OnMoveCommand(GridTile selectedGridTileToMoveTo)
    {
        base.OnMoveCommand(selectedGridTileToMoveTo);

        //if (directionToMoveAt == MapController.Directions.Top)
        //    currentPathToMoveThrough = new List<GridTile>(currentTopTileRoute);

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        isMoving = true;
    }

    public override void OnSelectedPiece()
    {
        base.OnSelectedPiece();

        currentTopTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.Top);
        currentBotTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.Bot);
        currentRightTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.Right);
        currentLeftTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 3, MapController.Directions.Left);
        currentTopRightTileRoute = MapController.Instance.GetPossibleRouteFromTile(standingOnTile, 5, MapController.Directions.TopRight);

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

        foreach (GridTile gridTile in currentTopRightTileRoute)
        {
            gridTile.DeactivateTile();
        }
    }
}
