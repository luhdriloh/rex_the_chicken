using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform _compassNeedle;

    private Transform _playerTransform;
    private Transform _gateTransform;

    private void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _gateTransform = GameObject.Find("Gate").transform;
    }

    private void Update()
    {
        Vector3 direction = _gateTransform.position - _playerTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _compassNeedle.rotation = rotation;
    }
}
