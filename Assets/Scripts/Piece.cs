using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GridTile standingOnTile;
    public float movementSpeed = 1f;

    public virtual void OnSelectedPiece()
    {

    }

    public virtual void OnDeselectedPiece()
    {

    }

    public virtual void OnMoveCommand(GridTile selectedGridTileToMoveTo)
    {

    }
}
