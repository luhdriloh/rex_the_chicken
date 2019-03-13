using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnEnemyDeathHandler(Vector3 position, float itemSpawnPercent);

public class LevelPlacer : MonoBehaviour
{
    public GameObject _miniAmmoBoxProto;
    public GameObject _weaponBox;
    public int _numberOfWeaponBoxes;
    public float _miniAmmoBoxDisapearTime;
    public List<GameObject> _enemyPrototypes;
    public List<float> _enemySpawnPercentages;

    public GameObject _player;
    public int _numberOfEnemies;

    private Inventory _playerInventory;
    private HashSet<int> _placesTaken;
    private Stack<MiniAmmoBox> _miniAmmoBoxes;
    private int _enemiesAlive;

    private void Awake()
    {
        _placesTaken = new HashSet<int>();
        _enemiesAlive = _numberOfEnemies;
        _playerInventory = _player.GetComponentInChildren<Inventory>();

        _miniAmmoBoxes = new Stack<MiniAmmoBox>();

        for (int i = 0; i < 4; i++)
        {
            GameObject ammoBoxObject = Instantiate(_miniAmmoBoxProto, transform.position, Quaternion.identity);
            ammoBoxObject.SetActive(false);

            MiniAmmoBox miniAmmoBox = ammoBoxObject.GetComponent<MiniAmmoBox>();
            miniAmmoBox._poolReturnDelegate = ReturnAmmoBox;
            miniAmmoBox._pickupAction = _playerInventory.AmmoPickup;
            _miniAmmoBoxes.Push(miniAmmoBox);
        }
    }


    private void DeathDelegate(Vector3 position, float itemSpawnPercent)
    {
        _enemiesAlive--;
        SpawnItem(position, itemSpawnPercent);
        if (_enemiesAlive <= 0)
        {
            Debug.Log("level done");
        }
    }

    private void SpawnItem(Vector3 position, float itemSpawnPercent)
    {
        // get weapon types

        //Dictionary<WeaponType, int> types = _playerInventory.GetWeaponTypes();
        //foreach (WeaponType type in types.Keys)
        //{
        //    Debug.Log(type + ": " + types[type]);
        //}


        // get weapon ammo percentage fills
        if (Random.value < itemSpawnPercent)
        {
            MiniAmmoBox miniAmmoBox = _miniAmmoBoxes.Pop();
            miniAmmoBox.SpawnAtPosition(position, _miniAmmoBoxDisapearTime);
        }
    }
    

    private void ReturnAmmoBox(GameObject miniAmmoBoxObject)
    {
        miniAmmoBoxObject.SetActive(false);
        _miniAmmoBoxes.Push(miniAmmoBoxObject.GetComponent<MiniAmmoBox>());
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
                    _placesTaken.Add(index);
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

        for (int i = 0; i < _numberOfWeaponBoxes; i++)
        {
            int index = Random.Range(0, innerTileList.Count - 1);
            Vector3 position = new Vector3(innerTileList[index].x, innerTileList[index].y, 0);
            Instantiate(_weaponBox, position, Quaternion.identity);
        }

        // set gate and player positions
        GameObject.Find("Gate").transform.position = new Vector3(endPositions[0].x * tileSize + tileSize / 2f, endPositions[0].y * tileSize + tileSize / 2f);
        _player.transform.position = new Vector3(tileSize/2f, tileSize / 2f, _player.transform.position.z);
    }
}
