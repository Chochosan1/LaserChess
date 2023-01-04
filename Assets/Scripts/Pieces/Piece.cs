using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public GridTile standingOnTile;
    public float movementSpeed = 1f;

    public abstract void OnSelectedPiece();


    public abstract void OnDeselectedPiece();


    public abstract void OnMoveCommand(GridTile selectedGridTileToMoveTo);
}
