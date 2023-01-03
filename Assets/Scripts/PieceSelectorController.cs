using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSelectorController : MonoBehaviour
{
    [SerializeField] private LayerMask pieceMask;
    [SerializeField] private LayerMask gridTileMask;

    [SerializeField] private Piece currentlySelectedPiece;

    //cache
    RaycastHit hit;
    Ray ray;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentlySelectedPiece == null)
                SelectPiece();
            else
                SelectTileAndMovePiece();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectCurrentPiece();
        }
    }

    public void SelectTileAndMovePiece()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridTileMask) && hit.collider.gameObject.GetComponent<GridTile>().IsTileActive)
        {
            currentlySelectedPiece.OnMoveCommand(hit.collider.gameObject.GetComponent<GridTile>());
            DeselectCurrentPiece();
        }
    }


    public void SelectPiece()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, pieceMask))
        {
            DeselectCurrentPiece();
            currentlySelectedPiece = hit.collider.gameObject.GetComponent<Piece>();
            currentlySelectedPiece.OnSelectedPiece();
        }
    }

    public void DeselectCurrentPiece()
    {
        if (currentlySelectedPiece == null)
            return;

        currentlySelectedPiece.OnDeselectedPiece();
        currentlySelectedPiece = null;
    }
}
