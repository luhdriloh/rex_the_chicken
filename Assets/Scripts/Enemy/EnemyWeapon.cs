using UnityEngine;

public class EnemyWeapon : Shooter
{
    public AudioClip _bulletSoundFX;
    public float _rotationSpeed;
    public float _startScale;

    private AudioSource _audioSource;
    private Quaternion _newRotation;

    protected override void Start()
    {
        base.Start();

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _bulletSoundFX;
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
}
