using System.Collections.Generic;
using UnityEngine;

public delegate void OnEnemyDeathHandler();

public class LevelPlacer : MonoBehaviour
{
    public List<GameObject> _fodderPrototypes;
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

        for (int i = 0; i < _numberOfEnemies; i++)
        {
            Vector2Int newPosition = Vector2Int.zero;
            bool taken = true;
            int index;

            while (taken)
            {
                index = Random.Range(0, innerTileList.Count - 1);
                if (_placesTaken.Contains(index) == false)
                {
                    taken = false;
                    newPosition = innerTileList[index];
                }
            }

            GameObject fodder = Instantiate(_fodderPrototypes[Random.Range(0, _fodderPrototypes.Count)]);
            fodder.GetComponent<IEnemy>().AddDeathDelegate(DeathDelegate);
            fodder.transform.position = new Vector3(newPosition.x, newPosition.y, 0);
        }

        Vector2Int playerposition = Vector2Int.zero;
        bool takenSpot = true;
        int indexToTake;

        while (takenSpot)
        {
            indexToTake = Random.Range(0, innerTileList.Count - 1);
            if (_placesTaken.Contains(indexToTake) == false)
            {
                takenSpot = false;
                playerposition = innerTileList[indexToTake];
            }
        }

        // set gate and player positions
        GameObject.Find("Gate").transform.position = new Vector3(endPositions[0].x * tileSize + tileSize / 2f, endPositions[0].y * tileSize + tileSize / 2f);
        _player.transform.position = new Vector3(tileSize/2f, tileSize / 2f, _player.transform.position.z);
    }
}
