using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectOnPlace : MonoBehaviour
{
    private static float _defaultZ = -3;

    private void Start()
    {
        float yPos = transform.position.y;
        transform.position = new Vector3(transform.position.x, transform.position.y, _defaultZ + (yPos * .01f));
    }
}
