using System.Collections.Generic;
using UnityEngine;

public class VisualEffectsPool : MonoBehaviour
{
    public static VisualEffectsPool _visualEffects;

    public GameObject _rexNotice;
    public GameObject _deadFodderPrototype;
    public int _numberOfFodderToCreate;

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

    }


    public GameObject GetObjectFromPool()
    {
        return null;
    }


    private void AddProjectilesToPool(int amountToAdd)
    {
    }
}
