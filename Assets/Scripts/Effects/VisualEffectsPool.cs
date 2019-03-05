using System.Collections.Generic;
using UnityEngine;

public class VisualEffectsPool : MonoBehaviour
{
    public static VisualEffectsPool _visualEffects;

    public GameObject _deadFodderPrototype;
    public int _numberOfFodderToCreate;

    private Stack<GameObject> _deadFoddersNotInUse;

    private void Start()
    {
        if (_visualEffects == null)
        {
            _visualEffects = this;
        }
        else if (_visualEffects != this)
        {
            Destroy(this);
        }

        // instantiate pool
        _deadFoddersNotInUse = new Stack<GameObject>();
        AddProjectilesToPool(_numberOfFodderToCreate);
    }

    public void PlaceDeadFodder(Vector3 location)
    {
        GameObject fodder = GetObjectFromPool();
        fodder.transform.position = location;
        fodder.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 48));
    }


    public GameObject GetObjectFromPool()
    {
        if (_deadFoddersNotInUse.Count == 0)
        {
            AddProjectilesToPool(3);
        }

        GameObject projectileToReturn = _deadFoddersNotInUse.Pop();
        projectileToReturn.gameObject.SetActive(true);
        return projectileToReturn;
    }


    private void AddProjectilesToPool(int amountToAdd)
    {

        for (int i = 0; i < amountToAdd; i++)
        {
            GameObject newGameObject = Instantiate(_deadFodderPrototype, transform.position, Quaternion.identity);
            _deadFoddersNotInUse.Push(newGameObject);

            // turn projectile off
            newGameObject.SetActive(false);
        }
    }
}
