using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public delegate void AmmoChangeHandler(Dictionary<WeaponType, int> ammoLeft);


public enum WeaponType
{ 
    LIGHT,
    SPECIAL,
    HEAVY
};


public class PlayerWeapon : Shooter, IItem
{
    public static event AmmoChangeHandler _ammoChangeEvent;
    public AudioClip _bulletSoundFX;
    public WeaponType _weaponType;
    public bool _pickup;
    public float _startScale;

    public SpriteRenderer _muzzleFlash;
    public int _muzzleFlashFrames;

    private bool _setRecoil;
    private RecoilScript _recoil;

    private static Dictionary<WeaponType, int> _weaponAmmo;
    private static Dictionary<WeaponType, int> _weaponAmmoMax = new Dictionary<WeaponType, int>
    {
        { WeaponType.LIGHT, 384 },
        { WeaponType.SPECIAL, 96 },
        { WeaponType.HEAVY, 64 }
    };

    private static Dictionary<WeaponType, int> _weaponPickupAmount = new Dictionary<WeaponType, int>
    {
        { WeaponType.LIGHT, 48 },
        { WeaponType.SPECIAL, 8 },
        { WeaponType.HEAVY, 6 }
    };

    protected override void Start()
    {
        base.Start();

        // start with some ammo
        _pickup = true;

        _recoil = GetComponent<RecoilScript>();

        // set up weapon ammo
        if (_weaponAmmo == null)
        {
            _weaponAmmo = new Dictionary<WeaponType, int>
            {
                { WeaponType.LIGHT, 96 },
                { WeaponType.SPECIAL, 12 },
                { WeaponType.HEAVY, 8 }
            };
        }

        OnAmmoChangeEvent();
    }


    private void OnDisable()
    {
        _setRecoil = false;
    }


    private void OnEnable()
    {
        OnAmmoChangeEvent();
    }


    private void Update()
    {
        if (_pickup == true)
        {
            return;
        }

        if (_setRecoil == false)
        {
            CameraFunctions._cameraFunctions.ChangeRecoilValues(_recoil._recoilAcceleration, _recoil._weaponRecoilStartSpeed, _recoil._maximumOffsetDistance);
            _setRecoil = true;
        }

        // get angle that you are pointing towards
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        RotateWeapon(angle);

        _currentTimeBetweenShotFired += Time.deltaTime;

        // fire weapon
        if ((Input.GetMouseButtonDown(0) && _currentTimeBetweenShotFired >= _tapFireDelay || Input.GetMouseButton(0) && _currentTimeBetweenShotFired >= _fireDelay))
        {
            if (_weaponAmmo[_weaponType] > 0)
            {
                Fire(angle);
            }
        }
    }


    private void Fire(float angle)
    {
        AudioEffects._audioEffects.PlaySoundEffect(_bulletSoundFX);
        _recoil.AddRecoil(transform.right);
        CameraFunctions._cameraFunctions.AddRecoil(transform.right);
        FireWeapon(angle);
        StartCoroutine("EmitMuzzleFlash");
        _weaponAmmo[_weaponType]--;
        OnAmmoChangeEvent();
    }

    private IEnumerator EmitMuzzleFlash()
    {
        _muzzleFlash.enabled = true;

        for (int i = 0; i < _muzzleFlashFrames; i++)
        {
            yield return 0;
        }

        _muzzleFlash.enabled = false;
    }

    private void RotateWeapon(float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        // switch orientation of weapon depending on where you are pointing
        transform.localScale = transform.right.x <= 0
            ? (Vector3)new Vector2(transform.transform.localScale.x, -_startScale)
            : (Vector3)new Vector2(transform.transform.localScale.x, _startScale);

        if (transform.right.y >= 0)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -.001f);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, .001f);
        }

    }


    public static void AddAmmo(Dictionary<WeaponType, int> weaponTypes)
    {
        // find weapons you have that are not full

        // it will go light, special, heavy
        List<WeaponType> typesOfWeapons = new List<WeaponType> { WeaponType.LIGHT, WeaponType.SPECIAL, WeaponType.HEAVY };
        Dictionary<WeaponType, float> selectionPercentages = new Dictionary<WeaponType, float>
        {
            { WeaponType.LIGHT, 0f },
            { WeaponType.SPECIAL, 0f },
            { WeaponType.HEAVY, 0f }
        };

        int totalAdded = 0;
        foreach (WeaponType type in weaponTypes.Keys)
        {
            if (_weaponAmmo[type] < _weaponAmmoMax[type])
            {
                for (int i = 0; i < weaponTypes[type]; i++)
                {
                    selectionPercentages[type] += .5f;
                    totalAdded++;
                }
            }
        }

        float toAdd = (1 - totalAdded * .5f) / 3f;
        if (totalAdded < 2)
        {
            typesOfWeapons.ForEach(type => selectionPercentages[type] += toAdd);
        }

        float randomValue = Random.value;
        float ammoChance = 0f;
        foreach (WeaponType type in typesOfWeapons)
        {
            ammoChance += selectionPercentages[type];
            if (randomValue < ammoChance)
            {
                _weaponAmmo[type] = Mathf.Min(_weaponAmmo[type] + _weaponPickupAmount[type], _weaponAmmoMax[type]);
                break;
            }
        }

        OnAmmoChangeEvent();
    }

    public static Dictionary<WeaponType, int> GetWeaponAmmoMax()
    {
        return _weaponAmmoMax;
    }


    public static void AddAmmoChangeHandler(AmmoChangeHandler handler)
    {
        _ammoChangeEvent += handler;
    }


    public static void OnAmmoChangeEvent()
    {
        if (_ammoChangeEvent != null)
        {
            _ammoChangeEvent(_weaponAmmo);
        }
    }


    public void SetAsActiveItem()
    {
        _pickup = false;
        transform.localScale = (Vector3)new Vector2(Mathf.Abs(_startScale), Mathf.Abs(_startScale));
        transform.localPosition = new Vector3(0, 0, transform.position.z);
        OnAmmoChangeEvent();
        gameObject.SetActive(true);
    }


    public void SetAsInactiveItem()
    {
        _pickup = false;
        gameObject.SetActive(false);
    }


    public void SetAsInventory(bool inventoryItem)
    {
        _pickup = !inventoryItem;
        GetComponent<BoxCollider2D>().enabled = !inventoryItem;
    }
}
