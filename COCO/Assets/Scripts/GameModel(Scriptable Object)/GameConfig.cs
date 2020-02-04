using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "COCO/GameConfig")]
public class GameConfig : ScriptableObject
{
    public int helpCost;
    public int initialCoins;
    public int initialLevel;
    public int levelPrize;
}
