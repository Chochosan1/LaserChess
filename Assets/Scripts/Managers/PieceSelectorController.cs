using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Takes care of the player's actions. Piece selection, player turn end, etc.
/// </summary>
public class PieceSelectorController : MonoBehaviour
{
    [Header("Masks")]
    [SerializeField] private LayerMask pieceMask;
    [SerializeField] private LayerMask gridTileMask;

    private Piece currentlySelectedPiece;

    //cache
    RaycastHit hit;
    Ray ray;

    private void Update()
    {
        //disable player control while in AI turn
        if (GameStateManager.Instance.CurrentState == GameStateManager.States.AITurn)
            return;

        //on click select a piece or a tile
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectPiece();
            SelectTileAndMovePiece();
        }

        //deselect the selected piece
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectCurrentPiece();
        }
    }

    //select an active tile and command the piece to move there
    private void SelectTileAndMovePiece()
    {
        if (currentlySelectedPiece == null)
            return;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridTileMask) && hit.collider.gameObject.GetComponent<GridTile>().IsTileActive)
        {
            currentlySelectedPiece.OnMoveCommand(hit.collider.gameObject.GetComponent<GridTile>());
            DeselectCurrentPiece();
        }
    }

    //select a piece
    private void SelectPiece()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, pieceMask))
        {
            DeselectCurrentPiece(); //deselect the already selected piece if there's one
            currentlySelectedPiece = hit.collider.gameObject.GetComponent<Piece>();
            currentlySelectedPiece.OnSelectedPiece();
        }
    }

    //deselects the current piece (if there's one) and clears everything associated with it
    public void DeselectCurrentPiece()
    {
        if (currentlySelectedPiece == null)
            return;

        currentlySelectedPiece.OnDeselectedPiece();
        currentlySelectedPiece = null;
    }
}
