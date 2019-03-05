using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelCreationData.asset", menuName = "LevelCreationData/Level Data", order = 1)]
public class LevelCreationData : ScriptableObject
{
    // level create details
    public bool _overlapAllowed;
    public int _tileSize;
    public int _numberOfWalkers;
    public int _numberOfIterations;

    // enemy details
    //public int _numberOfEnemies;
    //public List<GameObject> _enemyPrototypes;

    // item details
    //public List<GameObject> _weaponsList;
    //public List<int> _weaponRates;

    //public float _maxGunBoxesToSpawn;
    //public float _gunBoxSpawnPercentage;
}
