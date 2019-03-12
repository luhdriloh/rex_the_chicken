using System.Collections.Generic;
using UnityEngine;

public delegate void OnEnemyDeathHandler();

public class LevelPlacer : MonoBehaviour
{
    public List<GameObject> _enemyPrototypes;
    public List<float> _enemySpawnPercentages;

    public GameObject _player;
    public int _numberOfEnemies;

    private HashSet<int> _placesTaken;
    private int _enemiesAlive;


    private void Awake()
    {
        _placesTaken = new HashSet<int>();
        _enemiesAlive = _numberOfEnemies;
    }


    private void DeathDelegate()
    {
        _enemiesAlive--;
        if (_enemiesAlive <= 0)
        {
            Debug.Log("level done");
        }
    }


    public void PlaceThings(HashSet<Vector2Int> innerTiles, List<Vector2Int> endPositions, int tileSize)
    {
        List<Vector2Int> innerTileList = new List<Vector2Int>(innerTiles);
        Vector2 playerposition = new Vector3(tileSize / 2f, tileSize / 2f);

        for (int i = 0; i < _numberOfEnemies; i++)
        {
            Vector2Int newPosition = Vector2Int.zero;
            bool taken = true;
            int index;

            while (taken || (newPosition - playerposition).magnitude < 6)
            {
                taken = true;
                index = Random.Range(0, innerTileList.Count - 1);
                if (_placesTaken.Contains(index) == false)
                {
                    taken = false;
                    newPosition = innerTileList[index];
                }
            }

            float percentage = Random.value;
            int whichTypeOfEnemy = 0;
            float sum = 0;

            for (int enemyProtoIndex = 0; enemyProtoIndex < _enemyPrototypes.Count; enemyProtoIndex++)
            {
                sum += _enemySpawnPercentages[enemyProtoIndex];
                if (percentage <= sum)
                {
                    whichTypeOfEnemy = enemyProtoIndex;
                    break;
                }
            }

            GameObject fodder = Instantiate(_enemyPrototypes[whichTypeOfEnemy]);
            fodder.GetComponent<IEnemy>().AddDeathDelegate(DeathDelegate);
            fodder.transform.position = new Vector3(newPosition.x, newPosition.y, -1);
        }

        // set gate and player positions
        GameObject.Find("Gate").transform.position = new Vector3(endPositions[0].x * tileSize + tileSize / 2f, endPositions[0].y * tileSize + tileSize / 2f);
        _player.transform.position = new Vector3(tileSize/2f, tileSize / 2f, _player.transform.position.z);
    }
}
