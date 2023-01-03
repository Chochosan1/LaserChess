using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material possibleRouteMaterial;

    public int G;
    public int H;
    public int F { get { return G + H; } }

    public bool isBlocked = false;

    //neighbours orthogonal
    public GridTile topNeighbour;
    public GridTile botNeighbour;
    public GridTile leftNeighbour;
    public GridTile rightNeighbour;

    //neighbours diagonal
    public GridTile topRightNeighbour;
    public GridTile botRightNeighbour;
    public GridTile topLeftNeighbour;
    public GridTile botLeftNeighbour;

    public GridTile Previous;
    public Vector3Int gridLocation => new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
    public Vector2Int grid2DLocation => new Vector2Int((int)transform.position.x, (int)transform.position.z);

    public MapController.Directions currentSelectionDirection = MapController.Directions.None;

    private MeshRenderer meshRend;
    private bool isTileActive = false;
    public bool IsTileActive => isTileActive;

    private void Start()
    {
        meshRend = GetComponent<MeshRenderer>();

        FillNeighbourTiles();
        DeactivateTile();
    }

    public void FillNeighbourTiles()
    {
        Vector2Int TileToCheck = new Vector2Int(grid2DLocation.x + 1, grid2DLocation.y);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            rightNeighbour = MapController.Instance.map[TileToCheck];
            //if (Mathf.Abs(MapController.Instance.map[TileToCheck].transform.position.z - MapController.Instance.map[originTile].transform.position.z) <= 1)
            //    surroundingTiles.Add(MapController.Instance.map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(grid2DLocation.x - 1, grid2DLocation.y);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            leftNeighbour = MapController.Instance.map[TileToCheck];
            //if (Mathf.Abs(MapController.Instance.map[TileToCheck].transform.position.z - MapController.Instance.map[originTile].transform.position.z) <= 1)
            //    surroundingTiles.Add(MapController.Instance.map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(grid2DLocation.x, grid2DLocation.y + 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            topNeighbour = MapController.Instance.map[TileToCheck];
            //if (Mathf.Abs(MapController.Instance.map[TileToCheck].transform.position.z - MapController.Instance.map[originTile].transform.position.z) <= 1)
            //    surroundingTiles.Add(MapController.Instance.map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(grid2DLocation.x, grid2DLocation.y - 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            botNeighbour = MapController.Instance.map[TileToCheck];
            //if (Mathf.Abs(MapController.Instance.map[TileToCheck].transform.position.z - MapController.Instance.map[originTile].transform.position.z) <= 1)
            //    surroundingTiles.Add(MapController.Instance.map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(grid2DLocation.x + 1, grid2DLocation.y + 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            topRightNeighbour = MapController.Instance.map[TileToCheck];
            //if (Mathf.Abs(MapController.Instance.map[TileToCheck].transform.position.z - MapController.Instance.map[originTile].transform.position.z) <= 1)
            //    surroundingTiles.Add(MapController.Instance.map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(grid2DLocation.x - 1, grid2DLocation.y - 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            botLeftNeighbour = MapController.Instance.map[TileToCheck];
            //if (Mathf.Abs(MapController.Instance.map[TileToCheck].transform.position.z - MapController.Instance.map[originTile].transform.position.z) <= 1)
            //    surroundingTiles.Add(MapController.Instance.map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(grid2DLocation.x + 1, grid2DLocation.y - 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            botRightNeighbour = MapController.Instance.map[TileToCheck];
            //if (Mathf.Abs(MapController.Instance.map[TileToCheck].transform.position.z - MapController.Instance.map[originTile].transform.position.z) <= 1)
            //    surroundingTiles.Add(MapController.Instance.map[TileToCheck]);
        }

        TileToCheck = new Vector2Int(grid2DLocation.x - 1, grid2DLocation.y + 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            topLeftNeighbour = MapController.Instance.map[TileToCheck];
            //if (Mathf.Abs(MapController.Instance.map[TileToCheck].transform.position.z - MapController.Instance.map[originTile].transform.position.z) <= 1)
            //    surroundingTiles.Add(MapController.Instance.map[TileToCheck]);
        }
    }

    public void DeactivateTile()
    {
        meshRend.material = normalMaterial;
        isTileActive = false;
    }

    public void ActivateTile()
    {
        meshRend.material = possibleRouteMaterial;
        isTileActive = true;
    }
}
