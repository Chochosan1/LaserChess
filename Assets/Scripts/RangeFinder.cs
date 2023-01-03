using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder : MonoBehaviour
{
    public List<GridTile> GetTilesInRange(Vector2Int location, int range)
    {
        var startingTile = MapController.Instance.map[location];
        var inRangeTiles = new List<GridTile>();
        int stepCount = 0;

        inRangeTiles.Add(startingTile);

        //Should contain the surroundingTiles of the previous step. 
        var tilesForPreviousStep = new List<GridTile>();
        tilesForPreviousStep.Add(startingTile);
        while (stepCount < range)
        {
            var surroundingTiles = new List<GridTile>();

            foreach (var item in tilesForPreviousStep)
            {
      //          surroundingTiles.AddRange(MapController.Instance.GetSurroundingTiles(new Vector2Int(item.gridLocation.x, item.gridLocation.y)));
            }

            inRangeTiles.AddRange(surroundingTiles);
            tilesForPreviousStep = surroundingTiles.Distinct().ToList();
            stepCount++;
        }

        return inRangeTiles.Distinct().ToList();
    }
}
