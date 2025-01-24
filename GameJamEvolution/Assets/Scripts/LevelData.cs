using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Levels/LevelData", order = 1)]

public class LevelData : ScriptableObject
{
    public List<Obstacle> obstaclesToSpawn;
    public int index;
}
