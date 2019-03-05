using System.Collections.Generic;
using System.Collections;

using UnityEngine;

public class EnemyFodderAI : MonoBehaviour, IDamager, IEnemy
{
    public Sprite _deadSprite;
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

    private bool PlayerInLineOfSight()
    {
        // create a filter for the raycast
        RaycastHit2D[] hits = new RaycastHit2D[1];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Player"));
        filter.useTriggers = true;

        // get target position
        Vector3 direction = _playerTransform.position - transform.position;
        int collidersHit = Physics2D.Raycast(transform.position, direction.normalized * _lengthOfLineOfSight, filter, hits);

        return collidersHit > 0;
    }

    private IEnumerator IdleMovement()
    {
        // move at random intervals
        while (true)
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

            yield return new WaitForSeconds(Random.Range(_minAimInterval, _maxAimInterval));

            for (int i = 0; i < 2; i++)
            {
                yield return new WaitForSeconds(_enemyWeapon.GetFireDelay());

                if (PlayerInView())
                {
                    _enemyWeapon.FireWeapon();
                }
            }

            yield return new WaitForSeconds(Random.Range(_minFireInterval, _maxFireInterval));
        }
    }

    private void Move(Vector3 direction)
    {
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
        StopCoroutine("IdleMovement");
        StopCoroutine("ShootInIntervals");
        _dead = true;
        _rigidbody.velocity = Vector2.zero;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));


        GetComponent<CircleCollider2D>().enabled = false;
        _animator.enabled = false;
        _spriteRenderer.sprite = _deadSprite;
        _deathHandler();
    }

    public void AddDeathDelegate(OnEnemyDeathHandler deathHandler)
    {
        if (_deathHandler == null)
        {
            _deathHandler = deathHandler;
        }
    }


    public void TakeDamage(int damageAmount)
    {
        _health -= damageAmount;

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
