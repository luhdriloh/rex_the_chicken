using System.Collections.Generic;
using System.Collections;

using UnityEngine;

public class EnemyFodderAI : MonoBehaviour, IDamager, IEnemy
{
    public Sprite _deadSprite;
    public GameObject _noticeGameobject;

    public EnemyWeapon _enemyWeapon;
    public int _health;
    public float _movementSpeed;
    public float _lengthOfLineOfSight;
    public float _howCloseCanPlayerBe;
    public float _idleMovementTime;

    public float _minFireInterval;
    public float _maxFireInterval;
    public float _minAimInterval;
    public float _maxAimInterval;
    public float _minIdleRestTime;
    public float _maxIdleRestTime;

    // notice for player
    private bool _setNoticeSprite;
    private float _noticeTime;
    private int _bulletsToFire; 

    private static OnEnemyDeathHandler _deathHandler;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private Transform _playerTransform;
    private Vector3 _directionToMoveIn;
    private Vector2 _directionToFire;
    private float _amountOfTimeToMoveFor;
    private bool _shooting;
    private bool _rightFacing;
    private bool _routinesStarted;
    private bool _dead;

	private void Start ()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
        _amountOfTimeToMoveFor = 0f;
        _shooting = true;
        _rightFacing = false;
        _dead = false;

        GetComponent<CircleCollider2D>().enabled = true;

        // set up the enemy weapon
        Object[] weaponsList = Resources.LoadAll("Prefabs/Enemy/EnemyWeapons/Stage1Weapons", typeof(GameObject));
        GameObject enemyWeapon = Instantiate(weaponsList[Random.Range(0, weaponsList.Length)]) as GameObject;
        enemyWeapon.transform.parent = transform;
        enemyWeapon.transform.localPosition = new Vector3(0f, 0f, .1f);

        _enemyWeapon = enemyWeapon.GetComponent<EnemyWeapon>();

        _setNoticeSprite = _enemyWeapon._setNotice;
        _noticeTime = _enemyWeapon._noticeTime;
        _bulletsToFire = _enemyWeapon._bulletsToFire;
    }

    private void Update ()
    {
        if (_dead)
        {
            return;
        }

        if (_routinesStarted == false)
        {
            StartCoroutine("IdleMovement");
            StartCoroutine("ShootInIntervals");
            _routinesStarted = true;
        }

        // action distance

        // run away distance


        // 1: get position away from player and determine actions
        // if you are in the same action 'node' then continue

        // actions
        //   idle
        //   shoot
        //   run away
        //   aim at player
        //   aim in player vicinity
    }

    private IEnumerator IdleMovement()
    {
        // move at random intervals
        while (_dead == false)
        {
            float randomAngle = Random.Range(0f, 359f);
            _directionToMoveIn = new Vector2(Mathf.Sin(Mathf.Deg2Rad * randomAngle), Mathf.Cos(Mathf.Deg2Rad * randomAngle));
            _amountOfTimeToMoveFor = Random.Range(0f, _idleMovementTime);
            _animator.SetBool("Moving", true);

            // set up movement speed and stop coroutine
            Move(_directionToMoveIn);
            yield return new WaitForSeconds(_amountOfTimeToMoveFor);
            StopMovement();

            yield return new WaitForSeconds(Random.Range(_minIdleRestTime, _maxIdleRestTime));
        }
    }

    private IEnumerator ShootInIntervals()
    {
        // move at random intervals
        while (_shooting)
        {
            _directionToFire = _playerTransform.position - transform.position;

            float angle = Mathf.Atan2(_directionToFire.y, _directionToFire.x) * Mathf.Rad2Deg;
            _enemyWeapon.SetNewWeaponRotation(angle);

            float fireWaitTime = Random.Range(_minAimInterval, _maxAimInterval);
            bool fire = PlayerInView();

            yield return new WaitForSeconds(fireWaitTime);
            if (_setNoticeSprite && PlayerInView())
            {
                _noticeGameobject.SetActive(true);
                yield return new WaitForSeconds(_noticeTime);
                _noticeGameobject.SetActive(false);
            }

            for (int i = 0; i < _bulletsToFire; i++)
            {
                if (fire)
                {
                    _enemyWeapon.FireWeapon();
                }
                yield return new WaitForSeconds(_enemyWeapon.GetFireDelay());
            }

            yield return new WaitForSeconds(Random.Range(_minFireInterval, _maxFireInterval));
        }
    }

    private void Move(Vector3 direction)
    {
        if (_dead)
        {
            return;
        }

        // flip sprite if needed
        if (direction.x > Mathf.Epsilon)
        {
            if (_rightFacing == false)
            {
                _rightFacing = true;
                _spriteRenderer.flipX = false;
            }
        }
        else if (direction.x < -Mathf.Epsilon)
        {
            if (_rightFacing)
            {
                _rightFacing = false;
                _spriteRenderer.flipX = true;
            }
        }

        _rigidbody.velocity = direction.normalized * _movementSpeed;
    }

    private void StopMovement()
    {
        _rigidbody.velocity = Vector2.zero;
        _animator.SetBool("Moving", false);
    }
    

    private void Die()
    {
        StopMovement();
        StopCoroutine("IdleMovement");
        StopCoroutine("ShootInIntervals");
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        _dead = true;

        // put down weapon
        _enemyWeapon.DropWeapon();

        // remove children
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));


        GetComponent<CircleCollider2D>().enabled = false;
        _animator.enabled = false;
        _spriteRenderer.sprite = _deadSprite;
        _deathHandler();
        _rigidbody.velocity = Vector2.zero;
    }

    public void AddDeathDelegate(OnEnemyDeathHandler deathHandler)
    {
        if (_deathHandler == null)
        {
            _deathHandler = deathHandler;
        }
    }


    public void TakeDamage(int damageAmount, Vector2 bulletDirection)
    {
        _health -= damageAmount;
        _rigidbody.MovePosition(transform.position + (Vector3)(bulletDirection.normalized * .2f));

        if (_health <= 0)
        {
            Die();
        }
    }


    private bool PlayerInView()
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];

        // create a filter for the raycast
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Player", "MapEdge"));

        // the colliders hit will be stored in hits
        int collidersHit = Physics2D.Raycast(transform.position, _playerTransform.position - transform.position, filter, hits);

        return collidersHit == 1 && hits[0].collider.gameObject.layer == LayerMask.NameToLayer("Player");
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 inNormal = collision.contacts[0].normal;
        Vector2 newVelocity = Vector2.Reflect(_directionToMoveIn, inNormal);
        Move(newVelocity);
    }
}
