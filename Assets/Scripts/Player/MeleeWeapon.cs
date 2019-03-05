using UnityEngine;

public class MeleeWeapon : MonoBehaviour, IItem
{
    public GameObject _hitArc;
    public GameObject _weapon;

    public float _meleeReadyPositionAngle;
    public float _attackRotationSpeed;
    public float _fireDelay;
    // grab the swing arc object and play it

    private CircleCollider2D _hitArcCollider;
    private Animator _arcAnimator;
    private float _anglesToMove;
    private float _anglesMoved;
    private float _timeMovingBack;
    private bool _inSwing;

    private bool _movingBack;

    private void Start()
    {
        _hitArcCollider = _hitArc.GetComponent<CircleCollider2D>();
        _arcAnimator = _hitArc.GetComponent<Animator>();

        _anglesToMove = 180f;
        _anglesMoved = 0f;
        _timeMovingBack = 0f;
        _inSwing = false;
        _movingBack = false;
    }

    private void Update()
    {
        // if we swing and we are ready to swing, then swing
        // get the angle of the swing and rotate x angles
        // place the swing arc (damager) in the direction of the
        // mouse click
        // get angle that you are pointing towards
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        RotateWeapon(angle);
        HandleSwing();
    }

    private void RotateWeapon(float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;

        // switch orientation of weapon depending on where you are pointing
        transform.localScale = transform.right.x <= 0
            ? (Vector3)new Vector2(transform.transform.localScale.x, -transform.transform.localScale.x)
            : (Vector3)new Vector2(transform.transform.localScale.x, transform.transform.localScale.x);

        if (transform.right.y >= 0)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -.001f);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, .001f);
        }

    }


    private void HandleSwing()
    {
        if (Input.GetMouseButtonDown(0) && _inSwing == false)
        {
            _inSwing = true;
            _hitArcCollider.enabled = true;
            _arcAnimator.SetTrigger("Swing");
        }

        if (_inSwing)
        {
            if (_movingBack == false)
            {
                float rotationAddition = -_attackRotationSpeed * Time.deltaTime;
                _anglesMoved += -rotationAddition;

                _weapon.transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + rotationAddition);


                if (_anglesMoved > _anglesToMove)
                {
                    _hitArcCollider.enabled = false;
                    _movingBack = true;
                    return;
                }
            }
            else if (_movingBack == true)
            {
                _timeMovingBack += Time.deltaTime;
                float percent = _timeMovingBack / _fireDelay;
                float angles = Mathf.Lerp(_anglesMoved, -_meleeReadyPositionAngle, percent);

                _weapon.transform.localEulerAngles = new Vector3(0, 0, -angles);

                if (percent >= 1)
                {
                    _anglesMoved = 0f;
                    _timeMovingBack = 0f;
                    _movingBack = false;
                    _inSwing = false;
                }
            }
        }
    }

    public void SetAsActiveItem()
    {
        transform.localPosition = new Vector3(0, 0, transform.position.z);
        gameObject.SetActive(true);
    }

    public void SetAsInactiveItem()
    {
        gameObject.SetActive(false);
    }

    public Sprite GetItemSprite()
    {
        return _weapon.GetComponent<SpriteRenderer>().sprite;
    }
}
