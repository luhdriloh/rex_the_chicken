using UnityEngine;

public delegate void AmmoChangeHandler(int currentMagazineAmmo, int ammoLeft);

public enum WeaponType
{ 
    MACHINGUN,
    SNIPER,
    SHOTGUN
};


public class PlayerWeapon : Shooter, IItem
{
    public static event AmmoChangeHandler _ammoChangeEvent;
    public AudioClip _bulletSoundFX;
    public bool _pickup;
    public float _startScale;

    private int _ammoLeft;
    private int _ammoInMagazine;
    private bool _reloading;
    private bool _setRecoil;
    private float _reloadStartTime;
    private AudioSource _audioSource;
    private RecoilScript _recoil;

    protected override void Start()
    {
        base.Start();

        // start with half your max ammo
        _ammoLeft = _shooterStats._ammoCapacity / 2;
        _ammoInMagazine = _shooterStats._magazineSize;
        _reloading = false;
        _pickup = true;

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _bulletSoundFX;
        _recoil = GetComponent<RecoilScript>();
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
            if (_ammoInMagazine > 0)
            {
                if (_reloading == true)
                {
                    _reloading = false;
                }

                Fire(angle);
            }
            else
            {
                Reload();
            }
        }


        // reload
        if (Input.GetKeyDown(KeyCode.R) || _reloading == true)
        {
            Reload();
        }
    }


    private void Fire(float angle)
    {
        _audioSource.PlayOneShot(_bulletSoundFX);
        _recoil.AddRecoil(transform.right);
        CameraFunctions._cameraFunctions.AddRecoil(transform.right);
        FireWeapon(angle);
        _ammoInMagazine--;
        OnAmmoChangeEvent();
    }


    private void Reload()
    {
        // check that we have less than mag capacity in our current mag and that we have bullets left
        if (_ammoInMagazine >= _shooterStats._magazineSize || _ammoLeft <= 0)
        {
            return;
        }

        if (_reloading == false)
        {
            _reloading = true;
            _reloadStartTime = Time.time;
        }
        else if (_reloading == true)
        {
            float timeSinceReloadStart = Time.time - _reloadStartTime;
            if (timeSinceReloadStart >= _shooterStats._reloadTime)
            {
                // check that we have ammo left
                int ammoToAdd = _shooterStats._usesMagazine ? Mathf.Min(_ammoLeft, _shooterStats._magazineSize - _ammoInMagazine) :  Mathf.Min(_ammoLeft, 1);
                _ammoLeft -= ammoToAdd;
                _ammoInMagazine += ammoToAdd;

                // check if we have full ammo or out of ammo
                if (_ammoInMagazine >= _shooterStats._magazineSize || _ammoLeft <= 0)
                {
                    _reloading = false;
                }
                else
                {
                    _reloadStartTime = Time.time;
                }

                OnAmmoChangeEvent();
            }
        }
    }


    private void ThrowWeapon()
    { 
        
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


    public static void AddAmmoChangeHandler(AmmoChangeHandler handler)
    {
        _ammoChangeEvent += handler;
    }


    public void OnAmmoChangeEvent()
    {
        if (_ammoChangeEvent != null)
        {
            _ammoChangeEvent(_ammoInMagazine, _ammoLeft);
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
