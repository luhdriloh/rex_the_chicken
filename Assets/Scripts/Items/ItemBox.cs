using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public Sprite _openItemBox;

    private SpriteRenderer _spriteRenderer;
    private List<GameObject> items;

    private void Start()
    {
        items = new List<GameObject>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        Object[] weaponsList = Resources.LoadAll("Prefabs/PlayerWeapons/Stage1Weapons", typeof(GameObject));
        GameObject myObj = Instantiate(weaponsList[Random.Range(0, weaponsList.Length)]) as GameObject;
        myObj.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 359));
        myObj.transform.position = (Vector2)transform.position + new Vector2(Random.Range(-.5f, .5f), Random.Range(-1f, -2f));
        myObj.GetComponent<PlayerWeapon>()._pickup = true;
        myObj.SetActive(false);

        items.Add(myObj);
    }

    private void OnTriggerEnter2D()
    {
        _spriteRenderer.sprite = _openItemBox;
        GetComponent<BoxCollider2D>().enabled = false;

        items.ForEach(item => item.SetActive(true));
    }
}
