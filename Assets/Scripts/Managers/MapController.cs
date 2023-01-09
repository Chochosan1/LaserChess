using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public enum Directions { Top, Bot, Right, Left, TopRight, TopLeft, BotRight, BotLeft }
    public enum KnightPattern { KnightTopRight1, KnightTopRight2, KnightTopLeft1, KnightTopLeft2, KnightBotRight1, KnightBotRight2, KnightBotLeft1, KnightBotLeft2 }


    private static MapController instance;
    public static MapController Instance => instance;

    [Header("Grid Tiles")]
    [SerializeField] private List<GridTile> allGridTiles;

    public Dictionary<Vector2Int, GridTile> map = new Dictionary<Vector2Int, GridTile>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        //map all grid tiles by using their position as the key
        foreach (GridTile gridTile in allGridTiles)
        {
            map.Add(gridTile.grid2DLocation, gridTile);
        }
    }

    
    /// <summary>Returns the path/route in a certain direction starting from an initial tile. Encountering a taken/blocked tile will cut the path up to that point UNLESS ignoreBlockedTiles is true. Path length accepted as an argument.</summary>
    public List<GridTile> GetPossibleRouteFromTile(GridTile startTile, int routeLength, Directions direction, bool ignoreBlockedTiles = false)
    {
        List<GridTile> currentPossibleRoute = new List<GridTile>();

        GridTile currentTile = startTile; //use it as the current tile (node) to traverse; adding that current tile (node) to the path should reassign this var in order to traverse the next tile (node)
        bool routeBlocked = false; //use to stop traversing in the current direction if the path is blocked

        for (int i = 0; i < routeLength; i++)
        {
            if (routeBlocked)
                break;

            switch (direction)
            {
                case Directions.Top:
                    //add the first top neighbour tile to the path, then traverse its top neighbour and so on and on depending on path/route length (they act as separate nodes)
                    if (currentTile.TopNeighbour != null)
                    {
                        if (currentTile.TopNeighbour.IsBlocked && !ignoreBlockedTiles) //ignoreBlockedTiles = false will exclude all blocked tiles from the path (useful for movement paths). Attack paths on the other hand should return even blocked ones as enemies stand on blocked tiles 
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.TopNeighbour); //add the current tile (node)
                        currentTile = currentTile.TopNeighbour; //reassign the tile (node) to traverse from
                    }
                    break;
                case Directions.Bot:
                    if (currentTile.BotNeighbour != null)
                    {
                        if (currentTile.BotNeighbour.IsBlocked && !ignoreBlockedTiles)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.BotNeighbour);
                        currentTile = currentTile.BotNeighbour;
                    }
                    break;
                case Directions.Right:
                    if (currentTile.RightNeighbour != null)
                    {
                        if (currentTile.RightNeighbour.IsBlocked && !ignoreBlockedTiles)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.RightNeighbour);
                        currentTile = currentTile.RightNeighbour;
                    }
                    break;
                case Directions.Left:
                    if (currentTile.LeftNeighbour != null)
                    {
                        if (currentTile.LeftNeighbour.IsBlocked && !ignoreBlockedTiles)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.LeftNeighbour);
                        currentTile = currentTile.LeftNeighbour;
                    }
                    break;
                case Directions.TopRight:
                    if (currentTile.TopRightNeighbour != null)
                    {
                        if (currentTile.TopRightNeighbour.IsBlocked && !ignoreBlockedTiles)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.TopRightNeighbour);
                        currentTile = currentTile.TopRightNeighbour;
                    }
                    break;
                case Directions.TopLeft:
                    if (currentTile.TopLeftNeighbour != null)
                    {
                        if (currentTile.TopLeftNeighbour.IsBlocked && !ignoreBlockedTiles)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.TopLeftNeighbour);
                        currentTile = currentTile.TopLeftNeighbour;
                    }
                    break;
                case Directions.BotRight:
                    if (currentTile.BotRightNeighbour != null)
                    {
                        if (currentTile.BotRightNeighbour.IsBlocked && !ignoreBlockedTiles)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.BotRightNeighbour);
                        currentTile = currentTile.BotRightNeighbour;
                    }
                    break;
                case Directions.BotLeft:
                    if (currentTile.BotLeftNeighbour != null)
                    {
                        if (currentTile.BotLeftNeighbour.IsBlocked && !ignoreBlockedTiles)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.BotLeftNeighbour);
                        currentTile = currentTile.BotLeftNeighbour;
                    }
                    break;
            }
        }

        return currentPossibleRoute;
    }

    /// <summary>Returns the path/route in a certain knight pattern starting from an initial tile. Blocked paths and length do not matter.</summary>
    public List<GridTile> GetPossibleKnightRouteFromTile(GridTile startTile, KnightPattern knightPattern)
    {
        List<GridTile> currentPossibleRoute = new List<GridTile>();
        Vector2Int tilePositionToCheck = new Vector2Int();

        switch (knightPattern)
        {
            case KnightPattern.KnightTopRight1:
                tilePositionToCheck = new Vector2Int(startTile.grid2DLocation.x + 1, startTile.grid2DLocation.y + 2);

                //if the position where the knight pattern ends (the targeted tile) exists AND it is not blocked/taken then add it to the route
                if (map.ContainsKey(tilePositionToCheck) && !map[tilePositionToCheck].IsBlocked)
                    currentPossibleRoute.Add(map[tilePositionToCheck]);
                break;
            case KnightPattern.KnightTopRight2:
                tilePositionToCheck = new Vector2Int(startTile.grid2DLocation.x + 2, startTile.grid2DLocation.y + 1);

                if (map.ContainsKey(tilePositionToCheck) && !map[tilePositionToCheck].IsBlocked)
                    currentPossibleRoute.Add(map[tilePositionToCheck]);
                break;
            case KnightPattern.KnightTopLeft1:
                tilePositionToCheck = new Vector2Int(startTile.grid2DLocation.x - 1, startTile.grid2DLocation.y + 2);

                if (map.ContainsKey(tilePositionToCheck) && !map[tilePositionToCheck].IsBlocked)
                    currentPossibleRoute.Add(map[tilePositionToCheck]);
                break;
            case KnightPattern.KnightTopLeft2:
                tilePositionToCheck = new Vector2Int(startTile.grid2DLocation.x - 2, startTile.grid2DLocation.y + 1);

                if (map.ContainsKey(tilePositionToCheck) && !map[tilePositionToCheck].IsBlocked)
                    currentPossibleRoute.Add(map[tilePositionToCheck]);
                break;
            case KnightPattern.KnightBotRight1:
                tilePositionToCheck = new Vector2Int(startTile.grid2DLocation.x + 2, startTile.grid2DLocation.y - 1);

                if (map.ContainsKey(tilePositionToCheck) && !map[tilePositionToCheck].IsBlocked)
                    currentPossibleRoute.Add(map[tilePositionToCheck]);
                break;
            case KnightPattern.KnightBotRight2:
                tilePositionToCheck = new Vector2Int(startTile.grid2DLocation.x + 1, startTile.grid2DLocation.y - 2);

                if (map.ContainsKey(tilePositionToCheck) && !map[tilePositionToCheck].IsBlocked)
                    currentPossibleRoute.Add(map[tilePositionToCheck]);
                break;
            case KnightPattern.KnightBotLeft1:
                tilePositionToCheck = new Vector2Int(startTile.grid2DLocation.x - 2, startTile.grid2DLocation.y - 1);

                if (map.ContainsKey(tilePositionToCheck) && !map[tilePositionToCheck].IsBlocked)
                    currentPossibleRoute.Add(map[tilePositionToCheck]);
                break;
            case KnightPattern.KnightBotLeft2:
                tilePositionToCheck = new Vector2Int(startTile.grid2DLocation.x - 1, startTile.grid2DLocation.y - 2);

                if (map.ContainsKey(tilePositionToCheck) && !map[tilePositionToCheck].IsBlocked)
                    currentPossibleRoute.Add(map[tilePositionToCheck]);
                break;
        }

        return currentPossibleRoute;
    }
}
