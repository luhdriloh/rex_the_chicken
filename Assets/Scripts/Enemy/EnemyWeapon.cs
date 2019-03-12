using UnityEngine;

public class EnemyWeapon : Shooter
{
    public GameObject _playerWeaponEquivalentProto;
    public AudioClip _bulletSoundFX;
    public float _rotationSpeed;
    public float _startScale;
    public float _weaponGenerationPercent;

    public bool _setNotice;
    public float _noticeTime;
    public int _bulletsToFire;

    private AudioSource _audioSource;
    private Quaternion _newRotation;
    private GameObject _weaponToDrop;
    private bool _dropped;

    protected override void Start()
    {
        base.Start();

        _weaponToDrop = null;
        if (Random.value < _weaponGenerationPercent)
        {
            _weaponToDrop = Instantiate(_playerWeaponEquivalentProto, transform.position, Quaternion.identity);
            _weaponToDrop.SetActive(false);
        }

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _bulletSoundFX;
        _dropped = false;
    }

    private void Update()
    {
        if (transform.rotation != _newRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _newRotation, Time.deltaTime * _rotationSpeed);

            // switch orientation of weapon depending on where you are pointing
            transform.localScale = transform.right.x <= 0
                ? (Vector3)new Vector2(transform.transform.localScale.x, -_startScale)
                : (Vector3)new Vector2(transform.transform.localScale.x, _startScale);
        }
    }

    public float GetFireDelay()
    {
        return _fireDelay;
    }

    public void FireWeapon()
    {
        _audioSource.PlayOneShot(_bulletSoundFX);
        FireWeapon(transform.eulerAngles.z);
    }

    public void RotateWeapon(float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        // switch orientation of weapon depending on where you are pointing
        transform.localScale = transform.right.x <= 0
            ? (Vector3)new Vector2(transform.transform.localScale.x, -_startScale)
            : (Vector3)new Vector2(transform.transform.localScale.x, _startScale);
    }

    public void SetNewWeaponRotation(float angle)
    { 
        _newRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void DropWeapon()
    {
        if (_weaponToDrop != null && _dropped == false)
        {
            _dropped = true;
            _weaponToDrop.transform.parent = null;
            _weaponToDrop.gameObject.transform.position = new Vector3(transform.position.x + Random.Range(-.5f, .5f), transform.position.y + Random.Range(-.5f, .5f), -1.1f);
            _weaponToDrop.gameObject.SetActive(true);
        }
    }
}
