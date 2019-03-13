using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject _pickupPrototype;
    public int _numberOfItems;
    public List<GameObject> _itemList;

    private BoxCollider2D _collider;
    private int _itemInUseIndex;

    public void ChangeInventorySize(int size)
    {
        _numberOfItems = size;
    }

    public Dictionary<WeaponType, int> GetWeaponTypes()
    {
        Dictionary<WeaponType, int> weaponTypes = new Dictionary<WeaponType, int>();

        for (int i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i] != null)
            {
                WeaponType type = _itemList[i].GetComponent<PlayerWeapon>()._weaponType;

                if (weaponTypes.ContainsKey(type) == false)
                {
                    weaponTypes.Add(type, 1);
                }
                else
                {
                    weaponTypes[type]++;
                }
            }
        }

        return weaponTypes;
    }


    public void AmmoPickup()
    {
        PlayerWeapon.AddAmmo(GetWeaponTypes());
    }



    private void Start()
    {
        _collider = GetComponentInParent<BoxCollider2D>();

        _itemInUseIndex = 0;

        if (_itemList.Count == 0)
        {
            _itemList = new List<GameObject>(_numberOfItems);

            for (int i = 0; i < _numberOfItems; i++)
            {
                _itemList.Add(null);
            }
        }
        else
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i] != null)
                {
                    _itemList[i].GetComponent<PlayerWeapon>().SetAsInventory(true);
                }
            }
        }
    }

    private void Update()
    {
        // pickup item
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickupItem();
        }

        // switch item
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchItem(1);
        }
    }

    private void SwitchItem(int indexesToSwitch)
    {
        int newItemInUseIndex = _itemInUseIndex;
        indexesToSwitch = indexesToSwitch <= -1 ? _numberOfItems - 1 : 1;

        // set new item as active
        for (int i = 0; i < _numberOfItems; i++)
        {
            newItemInUseIndex = (newItemInUseIndex + indexesToSwitch) % _numberOfItems;
            if (_itemList[newItemInUseIndex] != null)
            {
                break;
            }
        }

        if (_itemList[_itemInUseIndex] == null && newItemInUseIndex == _itemInUseIndex)
        {
            return;
        }

        // set old item as inactive
        if (_itemList[_itemInUseIndex] != null)
        {
            _itemList[_itemInUseIndex].GetComponent<IItem>().SetAsInactiveItem();
        }

        // set new item as active
        _itemInUseIndex = newItemInUseIndex;
        _itemList[_itemInUseIndex].GetComponent<IItem>().SetAsActiveItem();
    }

    private void PickupItem()
    {
        // first check if we have an empty spot
        // else replace the weapon with the current item slot
        PlayerWeapon itemPickup = CheckIfItemOverlap();
        if (itemPickup == null)
        {
            return;
        }

        int emptyWeaponSlot = -1;

        for (int i = 0; i < _numberOfItems; i++)
        {
            if (_itemList[i] == null)
            {
                emptyWeaponSlot = i;
                break;
            }
        }

        // we do not have an empty weapon slot
        if (emptyWeaponSlot == -1)
        {
            DropItem(_itemInUseIndex, itemPickup.transform.position);
        }
        else
        {
            if (_itemList[_itemInUseIndex] != null)
            {
                _itemList[_itemInUseIndex].GetComponent<PlayerWeapon>().SetAsInactiveItem();
            }
            _itemInUseIndex = emptyWeaponSlot;
        }

        _itemList[_itemInUseIndex] = itemPickup.gameObject;
        _itemList[_itemInUseIndex].transform.parent = transform;
        _itemList[_itemInUseIndex].GetComponent<PlayerWeapon>().SetAsInventory(true);
        _itemList[_itemInUseIndex].GetComponent<PlayerWeapon>().SetAsActiveItem();
    }

    private PlayerWeapon CheckIfItemOverlap()
    {
        // set up contact filter and results array
        ContactFilter2D colliderFilter = new ContactFilter2D();
        colliderFilter.SetLayerMask(LayerMask.GetMask("Item"));
        colliderFilter.useTriggers = true;

        Collider2D[] results = new Collider2D[1];

        // find collider overlap
        int numberOfContacts = _collider.OverlapCollider(colliderFilter, results);
        return numberOfContacts == 0 ? null : results[0].GetComponent<PlayerWeapon>();
    }

    private void DropItem(int indexToDrop, Vector3 position)
    {
        _itemList[indexToDrop].transform.parent = null;
        _itemList[indexToDrop].GetComponent<PlayerWeapon>().SetAsInventory(false);
        _itemList[indexToDrop].transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 359));
        _itemList[indexToDrop].transform.position = position;
        _itemList[indexToDrop] = null;
    }
}
