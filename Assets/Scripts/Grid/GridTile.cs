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

    private Piece blockingTilePiece;
    private MeshRenderer meshRend;
    private bool isTileActive = false;
    private bool isBlocked = false;
    public bool IsTileActive => isTileActive;
    public bool IsBlocked => isBlocked;
    public Piece BlockingTilePiece => blockingTilePiece;

    private void Start()
    {
        meshRend = GetComponent<MeshRenderer>();

        FillNeighbourTiles();
        DeactivateTile();
    }

    //find all neighbour tiles adjacent to this tile (includes orthogonal and diagonal direcitons)
    private void FillNeighbourTiles()
    {
        Vector2Int TileToCheck = new Vector2Int(grid2DLocation.x + 1, grid2DLocation.y);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            rightNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x - 1, grid2DLocation.y);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            leftNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x, grid2DLocation.y + 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            topNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x, grid2DLocation.y - 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            botNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x + 1, grid2DLocation.y + 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            topRightNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x - 1, grid2DLocation.y - 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            botLeftNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x + 1, grid2DLocation.y - 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            botRightNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x - 1, grid2DLocation.y + 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            topLeftNeighbour = MapController.Instance.map[TileToCheck];
        }
    }

    /// <summary>Marks the tile as inactive and changes its color to the inactive one (e.g grey).</summary> 
    public void DeactivateTile()
    {
        meshRend.material = normalMaterial;
        isTileActive = false;
    }


    /// <summary>Marks the tile as active and changes its color to the active one (e.g green).</summary> 
    public void ActivateTile()
    {
        meshRend.material = possibleRouteMaterial;
        isTileActive = true;
    }

    public void MarkTileAsBlocked(Piece pieceThatIsOnTheTile)
    {
        blockingTilePiece = pieceThatIsOnTheTile;
        isBlocked = true;
    }

    public void MarkTileAsFree()
    {
        blockingTilePiece = null;
        isBlocked = false;
    }
}
