using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LaserChess.Utilities
{
    public static class LayerUtilities
    {
        ///<summary>Returns true if the passed object is in the specific layer.</summary>
        public static bool IsObjectInLayer(GameObject objectToCheck, LayerMask layerToCheck)
        {
            return layerToCheck == (layerToCheck | (1 << objectToCheck.layer));
        }
    }
}