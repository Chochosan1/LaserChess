using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpship : Piece
{
    private List<GridTile> currentKnightTopRight1Route;
    private List<GridTile> currentKnightTopRight2Route;
    private List<GridTile> currentKnightBotRight1Route;
    private List<GridTile> currentKnightBotRight2Route;
    private List<GridTile> currentKnightTopLeft1Route;
    private List<GridTile> currentKnightTopLeft2Route;
    private List<GridTile> currentKnightBotLeft1Route;
    private List<GridTile> currentKnightBotLeft2Route;

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
        standingOnTile.MarkTileAsFree();

        currentGridTileToMoveTo = selectedGridTileToMoveTo;
        currentGridTileToMoveTo.MarkTileAsBlocked(this); //mark it as blocked immediately so that clicking on another unit won't show that tile as free while another unit is traveling to it
        isMoving = true;
    }

    public override void OnSelectedPiece()
    {
        currentKnightTopRight1Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightTopRight1);
        currentKnightTopRight2Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightTopRight2);
        currentKnightTopLeft1Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightTopLeft1);
        currentKnightTopLeft2Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightTopLeft2);
        currentKnightBotLeft1Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightBotLeft1);
        currentKnightBotLeft2Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightBotLeft2);
        currentKnightBotRight1Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightBotRight1);
        currentKnightBotRight2Route = MapController.Instance.GetPossibleKnightRouteFromTile(standingOnTile, MapController.KnightPattern.KnightBotRight2);


        foreach (GridTile gridTile in currentKnightTopRight1Route)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentKnightTopRight2Route)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentKnightTopLeft1Route)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentKnightTopLeft2Route)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentKnightBotLeft1Route)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentKnightBotLeft2Route)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentKnightBotRight1Route)
        {
            gridTile.ActivateTile();
        }

        foreach (GridTile gridTile in currentKnightBotRight2Route)
        {
            gridTile.ActivateTile();
        }
    }

    public override void OnDeselectedPiece()
    {
        foreach (GridTile gridTile in currentKnightTopRight1Route)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentKnightTopRight2Route)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentKnightTopLeft1Route)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentKnightTopLeft2Route)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentKnightBotLeft1Route)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentKnightBotLeft2Route)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentKnightBotRight1Route)
        {
            gridTile.DeactivateTile();
        }

        foreach (GridTile gridTile in currentKnightBotRight2Route)
        {
            gridTile.DeactivateTile();
        }
    }
}
