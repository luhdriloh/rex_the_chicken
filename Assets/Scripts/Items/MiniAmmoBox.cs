using System.Collections;
using UnityEngine;

public delegate void ReturnObjectToPoolDelegate(GameObject gameObject);
public delegate void OnPickup();

public class MiniAmmoBox : MonoBehaviour
{
    public ReturnObjectToPoolDelegate _poolReturnDelegate;
    public OnPickup _pickupAction;

    public void SpawnAtPosition(Vector3 position, float time)
    {
        float x = position.x + Random.Range(-1f, 1f);
        float y = position.y + Random.Range(-1f, 1f);

        transform.position = new Vector3(x, y, transform.position.z);
        gameObject.SetActive(true);
        StartCoroutine(ReturnToPool(time));
    }

    private void OnTriggerEnter2D()
    {
        _pickupAction();
        StopCoroutine("ReturnToPool");
        _poolReturnDelegate(gameObject);
    }

    private IEnumerator ReturnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        _poolReturnDelegate(gameObject);
    }
}
