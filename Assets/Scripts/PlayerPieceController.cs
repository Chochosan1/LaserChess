using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPieceController : MonoBehaviour
{
    public float speed;
    public Piece piece;

    private PathFinder pathFinder = new PathFinder();
    private List<GridTile> path;
    private bool isMoving;
    private void Start()
    {
        pathFinder = new PathFinder();

        path = new List<GridTile>();
        isMoving = false;
    }

    void LateUpdate()
    {
        if (piece == null)
            return;

        RaycastHit? hit = GetFocusedOnTile();

        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            if (hit.HasValue)
            {
                GridTile tile = hit.Value.collider.gameObject.GetComponent<GridTile>();

                //  if (!isMoving)
                //  {
                path = pathFinder.FindPath(piece.standingOnTile, tile, new List<GridTile>());

                //for (int i = 0; i < path.Count; i++)
                //{
                //    var previousTile = i > 0 ? path[i - 1] : piece.standingOnTile;
                //    var futureTile = i < path.Count - 1 ? path[i + 1] : null;
                //}
                //}

                foreach (var tile1 in path)
                {
                    Debug.Log($"Name of found path tile: {tile1.name}");
                }

                isMoving = true;

                //if (Input.GetMouseButtonDown(0))
                //{
                //    foreach (var tile1 in path)
                //    {
                //        Debug.Log($"Name of found path tile: {tile1.name}");
                //    }
                //    if (piece == null)
                //    {
                //        PositionCharacterOnLine(tile);
                //    }
                //    else
                //    {
                //        isMoving = true;
                //    }
                //}
            }
        }

        if (path.Count > 0 && isMoving)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
      //  Debug.Log("MOVING");
        var step = speed * Time.deltaTime;

       //  piece.transform.position = Vector3.MoveTowards(piece.transform.position, path[0].transform.position, step);
         piece.transform.position = Vector3.MoveTowards(piece.transform.position, new Vector3(path[0].transform.position.x, piece.transform.position.y, path[0].transform.position.z), step);
      //  piece.transform.position = new Vector3(piece.transform.position.x, 1f, path[0].transform.position.z);
      //   piece.transform.position = new Vector3(path[0].transform.position.x, 1f, path[0].transform.position.z);

        if (Vector3.Distance(piece.transform.position, new Vector3(path[0].transform.position.x, piece.transform.position.y, path[0].transform.position.z)) < 0.01f)
        {
            Debug.Log($"Tile {path[0].name}");
            PositionCharacterOnLine(path[0]);
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {
            //   GetInRangeTiles();
            isMoving = false;
        }

    }

    private void PositionCharacterOnLine(GridTile tile)
    {
        piece.transform.position = new Vector3(tile.transform.position.x, 1f, tile.transform.position.z);
        piece.standingOnTile = tile;
    }

    private static RaycastHit? GetFocusedOnTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
          //  Debug.Log(hit.transform.name);
            //Debug.Log("hit");
            return hit;
        }
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        //RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        //if (hits.Length > 0)
        //{
        //    return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        //}

        return null;
    }

    //private void GetInRangeTiles()
    //{
    //    rangeFinderTiles = rangeFinder.GetTilesInRange(new Vector2Int(piece.standingOnTile.gridLocation.x, piece.standingOnTile.gridLocation.z), 3);

    //    //foreach (var item in rangeFinderTiles)
    //    //{
    //    //    item.ShowTile();
    //    //}
    //}
}
