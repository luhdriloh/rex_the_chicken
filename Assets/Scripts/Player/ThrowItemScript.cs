using System.Collections;
using UnityEngine;

public class ThrowItemScript : MonoBehaviour
{
    public float _speed;
    public float _acceleration;

    private BoxCollider2D _boxcollider;
    private Rigidbody2D _rigidbody;
    private float _timeToStop;
    private bool _thrown;

    private void Start()
    {
        _boxcollider = GetComponent<BoxCollider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _timeToStop = _speed / _acceleration;
        _thrown = false;
    }

    public void Throw(Vector2 direction)
    {
        _thrown = true;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = direction.normalized * _speed;
        _boxcollider.isTrigger = false;
        StartCoroutine("StopWeapon");
    }

    private void FixedUpdate()
    {
        if (_thrown)
        {
            _rigidbody.velocity -= _rigidbody.velocity.normalized * _acceleration * Time.deltaTime;

            if (_rigidbody.velocity.magnitude <= Mathf.Epsilon)
            {
                ChangeWeaponToPickup();
            }
        }
    }

    private IEnumerator StopWeapon()
    {
        yield return new WaitForSeconds(_timeToStop);
        ChangeWeaponToPickup();
    }

    private void ChangeWeaponToPickup()
    {
        StopCoroutine("StopWeapon");
        _thrown = false;
        _rigidbody.isKinematic = true;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if it is not mapedge than we get the damager interface
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.GetComponent<IDamager>().TakeDamage(2, _rigidbody.velocity);
        }
    }
}
