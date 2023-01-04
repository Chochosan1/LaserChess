using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private Dictionary<Vector2Int, GridTile> searchableTiles;

    public List<GridTile> FindPath(GridTile start, GridTile end, List<GridTile> inRangeTiles)
    {
         searchableTiles = new Dictionary<Vector2Int, GridTile>();
      //  searchableTiles = MapController.Instance.map;

        List<GridTile> openList = new List<GridTile>();
        HashSet<GridTile> closedList = new HashSet<GridTile>();

        if (inRangeTiles.Count > 0)
        {
            foreach (var item in inRangeTiles)
            {
                searchableTiles.Add(item.grid2DLocation, MapController.Instance.map[item.grid2DLocation]);
            }
        }
        else
        {
            searchableTiles = MapController.Instance.map;
        }

        openList.Add(start);

        while (openList.Count > 0)
        {
            GridTile currentGridTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentGridTile);
            closedList.Add(currentGridTile);

            if (currentGridTile == end)
            {
                return GetFinishedList(start, end);
            }

            foreach (var tile in GetNeightbourGridTiles(currentGridTile))
            {
                if (tile.IsBlocked || closedList.Contains(tile) /*|| Mathf.Abs(currentGridTile.transform.position.z - tile.transform.position.z) > 1*/)
                {
                    continue;
                }

                tile.G = GetManhattenDistance(start, tile);
                tile.H = GetManhattenDistance(end, tile);

                tile.Previous = currentGridTile;


                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                    Debug.Log($"ADDING TILE TO PATH {tile.name}");
                }

             //   Debug.Log("Open LIST > 0");
            }
        }

        return new List<GridTile>();
    }

    private List<GridTile> GetFinishedList(GridTile start, GridTile end)
    {
        List<GridTile> finishedList = new List<GridTile>();
        GridTile currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.Previous;
        }

        finishedList.Reverse();

        return finishedList;
    }

    private int GetManhattenDistance(GridTile start, GridTile tile)
    {
        return Mathf.Abs(start.gridLocation.x - tile.gridLocation.x) + Mathf.Abs(start.gridLocation.z - tile.gridLocation.z);
    }

    private List<GridTile> GetNeightbourGridTiles(GridTile currentGridTile)
    {
        var map = MapController.Instance.map;

        List<GridTile> neighbours = new List<GridTile>();

        //right
        Vector2Int locationToCheck = new Vector2Int(
            currentGridTile.gridLocation.x + 1,
            currentGridTile.gridLocation.z
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        //left
        locationToCheck = new Vector2Int(
            currentGridTile.gridLocation.x - 1,
            currentGridTile.gridLocation.z
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        //top
        locationToCheck = new Vector2Int(
            currentGridTile.gridLocation.x,
            currentGridTile.gridLocation.z + 1
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        //bottom
        locationToCheck = new Vector2Int(
            currentGridTile.gridLocation.x,
            currentGridTile.gridLocation.z - 1
        );

        if (searchableTiles.ContainsKey(locationToCheck))
        {
            neighbours.Add(searchableTiles[locationToCheck]);
        }

        return neighbours;
    }
}
