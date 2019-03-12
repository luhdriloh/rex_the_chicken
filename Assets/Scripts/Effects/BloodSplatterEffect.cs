using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterEffect : MonoBehaviour
{
    public static BloodSplatterEffect _bloodSplatterEffect;

    public List<Sprite> _bloodSplatterSprites;
    public GameObject _bloodSplatter;
    public int _startPoolSize;

    private Stack<BloodSplatter> _bloodSplatters;


    private void Start()
    {
        if (_bloodSplatterEffect == null)
        {
            _bloodSplatterEffect = this;
            _bloodSplatters = new Stack<BloodSplatter>();
            AddBloodSplattersToPool(_startPoolSize);
        }
        else if (_bloodSplatterEffect != this)
        {
            Destroy(this);
        }
    }


    public void PlaceBloodSplatter(Vector3 position)
    {
        BloodSplatter bloodSplatter = GetObjectFromPool();
        bloodSplatter.SetBloodSplatter(position, Random.Range(0, 360), _bloodSplatterSprites[Random.Range(0, _bloodSplatterSprites.Count)]);
    }


    // POOL FUNCTIONALITY //

    public BloodSplatter GetObjectFromPool()
    {
        if (_bloodSplatters.Count == 0)
        {
            AddBloodSplattersToPool(10);
        }

        BloodSplatter bloodSplatter = _bloodSplatters.Pop();
        bloodSplatter.gameObject.SetActive(true);

        return bloodSplatter;
    }


    private void AddBloodSplattersToPool(int amountToAdd)
    {
        for (int i = 0; i < amountToAdd; i++)
        {
            GameObject newGameObject = Instantiate(_bloodSplatter, transform.position, Quaternion.identity);
            newGameObject.SetActive(false);
            _bloodSplatters.Push(newGameObject.GetComponent<BloodSplatter>());
        }
    }
}
