using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Stats", menuName = "LaserChess/Pieces/StatsAsset", order = 1)]
public class PieceStats : ScriptableObject
{
    [SerializeField] private int attackPower;
    [SerializeField] private int hitPoints;

    public int AttackPower => attackPower;
    public int HitPoints => hitPoints;
}
