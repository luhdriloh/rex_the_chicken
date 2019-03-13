using System.Collections;

using UnityEngine;

public class ChaserEnemyAI : MonoBehaviour, IDamager, IEnemy
{
    public Sprite _deadSprite;
    public int _health;
    public float _speed;
    public float _timeChasing;

    private static OnEnemyDeathHandler _deathHandler;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private static Transform _playerTransform;
    private bool _rightFacing;
    private bool _dead;
    private bool _startChase;


    public void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _rightFacing = false;
        _dead = false;
        _startChase = false;

        if (_playerTransform == null)
        {
            _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
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


    public void Update()
    {
        if (_dead == true)
        {
            return;
        }

        if (PlayerInView())
        {
            Move(_playerTransform.position - transform.position);
            _animator.SetBool("Moving", true);

            if (_startChase == true)
            {
                _startChase = false;
                StopCoroutine("StopMovement");
            }
        }
        else
        {
            if (_startChase == false)
            {
                _startChase = true;
                StartCoroutine(StopMovement(2));
            }
        }
    }


    public void AddDeathDelegate(OnEnemyDeathHandler deathHandler)
    {
        if (_deathHandler == null)
        {
            _deathHandler = deathHandler;
        }
    }


    private IEnumerator StopMovement(float time)
    {
        yield return new WaitForSeconds(time);
        _rigidbody.velocity = Vector2.zero;
        _animator.SetBool("Moving", false);
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

        _rigidbody.velocity = direction.normalized * _speed;
    }


    private void Die()
    {
        _rigidbody.velocity = Vector2.zero;
        _dead = true;
        GetComponent<Animator>().enabled = false;
        _spriteRenderer.sprite = _deadSprite;
        GetComponent<BoxCollider2D>().enabled = false;
        //_deathHandler();
    }

    private bool PlayerInView()
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];

        // create a filter for the raycast
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Player", "MapEdge"));
        filter.useTriggers = true;

        // the colliders hit will be stored in hits
        int collidersHit = Physics2D.Raycast(transform.position, _playerTransform.position - transform.position, filter, hits);

        return collidersHit == 1 && hits[0].collider.gameObject.layer == LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 inNormal = collision.contacts[0].normal;
        Vector2 newVelocity = Vector2.Reflect(_rigidbody.velocity, inNormal);
        Move(newVelocity);
    }
}