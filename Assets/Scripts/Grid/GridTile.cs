using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls the state and data for each tile. Each tile can be activated/deactivated and automatically finds all of its adjacent neighbours (includes orthogonal & diagonal)
/// </summary>
public class GridTile : MonoBehaviour
{
    [Header("AI Win Condition")]
    [Tooltip("Set to true if this tile should count towards the win condition of the AI")]
    [SerializeField] private bool countAsWinConditionOnReachedByAI = false;

    [Header("Materials")]
    [SerializeField] private Material possibleRouteMaterial;

    //neighbours orthogonal
    public GridTile TopNeighbour { get; set; }
    public GridTile botNeighbour { get; set; }
    public GridTile LeftNeighbour { get; set; }
    public GridTile RightNeighbour { get; set; }

    //neighbours diagonal
    public GridTile TopRightNeighbour { get; set; }
    public GridTile BotRightNeighbour { get; set; }
    public GridTile TopLeftNeighbour { get; set; }
    public GridTile BotLeftNeighbour { get; set; }

    public Vector3Int gridLocation => new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
    public Vector2Int grid2DLocation => new Vector2Int((int)transform.position.x, (int)transform.position.z);

    private Piece blockingTilePiece;
    private MeshRenderer meshRend;
    private Material normalMaterial;
    private bool isTileActive = false;
    private bool isBlocked = false;

    /// <summary>Has the tile been activated (e.g available for selection)?</summary>
    public bool IsTileActive => isTileActive;

    /// <summary>Is there anything standing on top of the tile?</summary>
    public bool IsBlocked => isBlocked;

    /// <summary>The piece that is blocking/occupying the tile.</summary>
    public Piece BlockingTilePiece => blockingTilePiece;

    /// <summary>Should this tile count as a win condition for the AI?</summary>
    public bool CountAsWinConditionOnReachedByAI => countAsWinConditionOnReachedByAI;

    private void Start()
    {
        meshRend = GetComponent<MeshRenderer>();
        normalMaterial = GetComponent<MeshRenderer>().material; //cache the original mat

        FillNeighbourTiles();
        DeactivateTile();
    }

    //find all neighbour tiles adjacent to this tile (includes orthogonal and diagonal direcitons)
    private void FillNeighbourTiles()
    {
        Vector2Int TileToCheck = new Vector2Int(grid2DLocation.x + 1, grid2DLocation.y);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            RightNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x - 1, grid2DLocation.y);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            LeftNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x, grid2DLocation.y + 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            TopNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x, grid2DLocation.y - 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            botNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x + 1, grid2DLocation.y + 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            TopRightNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x - 1, grid2DLocation.y - 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            BotLeftNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x + 1, grid2DLocation.y - 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            BotRightNeighbour = MapController.Instance.map[TileToCheck];
        }

        TileToCheck = new Vector2Int(grid2DLocation.x - 1, grid2DLocation.y + 1);
        if (MapController.Instance.map.ContainsKey(TileToCheck))
        {
            TopLeftNeighbour = MapController.Instance.map[TileToCheck];
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

    /// <summary>Marks the tile blocked/occupied by a piece.</summary> 
    public void MarkTileAsBlocked(Piece pieceThatIsOnTheTile)
    {
        blockingTilePiece = pieceThatIsOnTheTile;
        isBlocked = true;
    }

    /// <summary>Marks the tile as free (not blocked/occupied by any piece).</summary> 
    public void MarkTileAsFree()
    {
        blockingTilePiece = null;
        isBlocked = false;
    }
}
