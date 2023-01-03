using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public enum Directions { None, Top, Bot, Right, Left, TopRight, TopLeft, BotRight, BotLeft}

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

        foreach(GridTile gridTile in allGridTiles)
        {
            map.Add(gridTile.grid2DLocation, gridTile);
       //     Debug.Log($"ADDED TILE: {gridTile.name} with coords: {gridTile.grid2DLocation}");
        }
    }

    /// <summary>Returns the path/route in a certain direction starting from an initial tile. Encountering a taken/blocked tile will cut the path up to that point. Path length accepted as an argument.</summary>
    public List<GridTile> GetPossibleRouteFromTile(GridTile startTile, int routeLength, Directions direction)
    {
        List<GridTile> currentPossibleRoute = new List<GridTile>();

        bool routeBlocked = false;
        GridTile currentTile = startTile;

        for(int i = 0; i < routeLength; i++)
        {
            if (routeBlocked)
                break;

            switch (direction)
            {
                case Directions.Top:
                    if (currentTile.topNeighbour != null)
                    {
                        if (currentTile.topNeighbour.isBlocked)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.topNeighbour);
                        currentTile = currentTile.topNeighbour;
                    }
                    break;
                case Directions.Bot:
                    if (currentTile.botNeighbour != null)
                    {
                        if (currentTile.botNeighbour.isBlocked)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.botNeighbour);
                        currentTile = currentTile.botNeighbour;
                    }
                    break;
                case Directions.Right:
                    if (currentTile.rightNeighbour != null)
                    {
                        if (currentTile.rightNeighbour.isBlocked)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.rightNeighbour);
                        currentTile = currentTile.rightNeighbour;
                    }
                    break;
                case Directions.Left:
                    if (currentTile.leftNeighbour != null)
                    {
                        if (currentTile.leftNeighbour.isBlocked)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.leftNeighbour);
                        currentTile = currentTile.leftNeighbour;
                    }
                    break;
                case Directions.TopRight:
                    if (currentTile.topRightNeighbour != null)
                    {
                        if (currentTile.topRightNeighbour.isBlocked)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.topRightNeighbour);
                        currentTile = currentTile.topRightNeighbour;
                    }
                    break;
                case Directions.TopLeft:
                    if (currentTile.topLeftNeighbour != null)
                    {
                        if (currentTile.topLeftNeighbour.isBlocked)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.topLeftNeighbour);
                        currentTile = currentTile.topLeftNeighbour;
                    }
                    break;
                case Directions.BotRight:
                    if (currentTile.botRightNeighbour != null)
                    {
                        if (currentTile.botRightNeighbour.isBlocked)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.botRightNeighbour);
                        currentTile = currentTile.botRightNeighbour;
                    }
                    break;
                case Directions.BotLeft:
                    if (currentTile.botLeftNeighbour != null)
                    {
                        if (currentTile.botLeftNeighbour.isBlocked)
                        {
                            routeBlocked = true;
                            break;
                        }

                        currentPossibleRoute.Add(currentTile.botLeftNeighbour);
                        currentTile = currentTile.botLeftNeighbour;
                    }
                    break;
            }
        }

        return currentPossibleRoute;
    }
}
