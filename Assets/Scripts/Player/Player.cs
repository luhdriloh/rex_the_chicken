using System.Collections;

using UnityEngine;

public delegate void HealthChangeHandler(int newHealthValue, int maxHealthValue);

public class Player : MonoBehaviour, IDamager
{
    public event HealthChangeHandler _healthChangedEvent;

    // player stats
    public int _maxHealth;
    public int _health;
    public float _movementSpeed;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private ParticleSystem _walkDust;
    private bool _walking;
    private bool _rightFacing;
    private bool _roll;

    private void Awake()
    {
        // set up player ammo
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _walkDust = GetComponentInChildren<ParticleSystem>();
        _walking = false;
        _rightFacing = true;
    }

    private void Update()
    {
        if (_roll == false)
        {
            Move();
        }
        else
        {
            _walkDust.Emit(2);
        }
    }

    private void Move()
    {
        float xMovement = Input.GetAxisRaw("Horizontal");

        // flip sprite if needed
        if (xMovement > Mathf.Epsilon)
        {
            if (_rightFacing == false)
            {
                _rightFacing = true;
                _spriteRenderer.flipX = false;
            }
        }
        else if (xMovement < -Mathf.Epsilon)
        {
            if (_rightFacing)
            {
                _rightFacing = false;
                _spriteRenderer.flipX = true;
            }
        }

        // roll
        if (Input.GetMouseButtonDown(1) && _rigidbody.velocity.magnitude >= 1)
        {
            _animator.SetTrigger("Roll");
            StartCoroutine(StopRoll(.3f));
            _roll = true;
        }

        // set new player speed
        float speed = _movementSpeed + (_roll == true ? 2f : 0f);
        Vector2 movementInDirection = new Vector2(xMovement, Input.GetAxisRaw("Vertical")).normalized * speed;
        _rigidbody.velocity = movementInDirection;

        // set walking animation
        if (_walking == false && movementInDirection.magnitude >= 1)
        {
            _walkDust.Play();
            _animator.SetBool("Moving", true);
            _walking = true;
        }
        else if (_walking == true && movementInDirection.magnitude < 1)
        {
            _walkDust.Stop();
            _animator.SetBool("Moving", false);
            _walking = false;
        }

        // for trees and shit
        float yPos = transform.position.y;
        transform.position = new Vector3(transform.position.x, transform.position.y, -3f + (yPos * .01f));
    }


    public void AddHealthChangeSubscriber(HealthChangeHandler healthChangeHandler)
    {
        _healthChangedEvent += healthChangeHandler;
        OnHealthChange();
    }


    public void HealthChange(int healthAmount)
    {
        // change health based on if health is added or subtracted
        _health = healthAmount < 0
            ? Mathf.Max(_health + healthAmount, 0)
            : Mathf.Min(_health + healthAmount, _maxHealth);

        OnHealthChange();

        if (_health <= 0)
        {
            // death animation

            // destroy player
            Destroy(this);
        }
    }


    protected virtual void OnHealthChange()
    {
        if (_healthChangedEvent != null)
        {
            _healthChangedEvent(_health, _maxHealth);
        }
    }


    public void TakeDamage(int amountOfDamage, Vector2 bulletDirection)
    {
        _rigidbody.MovePosition(transform.position + (Vector3)(bulletDirection.normalized * .2f));
        BloodSplatterEffect._bloodSplatterEffect.PlaceBloodSplatter(transform.position + (Vector3)(bulletDirection.normalized * .4f) + Vector3.forward);
        HealthChange(-amountOfDamage);
    }

    private IEnumerator StopRoll(float time)
    {
        yield return new WaitForSeconds(time);
        _roll = false;
    }
}
