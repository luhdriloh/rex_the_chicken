using System.Collections.Generic;
using UnityEngine;

using ActionGameFramework.Projectiles;
using ActionGameFramework.Helpers;

public delegate void ReturnToPool(LinearProjectile projectile);

// public static ammo

public class Shooter : MonoBehaviour
{
    public bool _playerWeapon;
    public GameObject _weaponProjectile;
    public GameObject _firepointGameObject;
    public ShooterStats _shooterStats;
    public int _projectilesToCreate;

    protected float _fireDelay;
    protected float _tapFireDelay;
    protected float _currentTimeBetweenShotFired;

    private Stack<LinearProjectile> _projectilesNotInUse;
    private Transform _firepoint;

    protected virtual void Start()
    {
        _fireDelay = 60f / _shooterStats._rpmMax;
        _tapFireDelay = 60f / _shooterStats._rpmTapMax;

        _currentTimeBetweenShotFired = 100f;
        _firepoint = _firepointGameObject.transform;

        // instantiate pool
        _projectilesNotInUse = new Stack<LinearProjectile>();
        AddProjectilesToPool(_projectilesToCreate);
    }

    protected void FireWeapon(float angle)
    {
        for (int i = 0; i < _shooterStats._projectilesPerShot; i++)
        {
            LinearProjectile projectile = GetObjectFromPool();
            float bulletAngleOfTravel = Random.Range(-_shooterStats._recoilInDegrees, _shooterStats._recoilInDegrees) + angle;

            projectile.FireInDirection(_firepoint.position, BulletTravelVector(bulletAngleOfTravel), bulletAngleOfTravel);
        }

        _currentTimeBetweenShotFired = 0f;
    }

    protected void FireLeadingShot(Vector2 target, Vector2 velocity)
    {
        for (int i = 0; i < _shooterStats._projectilesPerShot; i++)
        {
            LinearProjectile projectile = GetObjectFromPool();
            Vector3 targetLeadFireVector = Ballistics.CalculateLinearLeadingTargetPoint(transform.position, target, velocity, projectile._startSpeed, projectile._acceleration);

            projectile.FireAtPoint(_firepoint.position, targetLeadFireVector);
        }

        _currentTimeBetweenShotFired = 0f;
    }

    private Vector3 BulletTravelVector(float bulletAngleOfTravel)
    {
        return new Vector3(Mathf.Cos(Mathf.Deg2Rad * bulletAngleOfTravel), Mathf.Sin(Mathf.Deg2Rad * bulletAngleOfTravel), 0);
    }


    // POOL FUNCTIONALITY // 

    public void HandleReturnToPool(LinearProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _projectilesNotInUse.Push(projectile);
    }


    public LinearProjectile GetObjectFromPool()
    {
        if (_projectilesNotInUse.Count == 0)
        {
            AddProjectilesToPool(3);
        }

        LinearProjectile projectileToReturn = _projectilesNotInUse.Pop();
        projectileToReturn.gameObject.SetActive(true);
        return projectileToReturn;
    }


    private void AddProjectilesToPool(int amountToAdd)
    {
        int layerToSet = _playerWeapon == true ? LayerMask.NameToLayer("Projectile") : LayerMask.NameToLayer("EnemyProjectile");

        GameObject newGameObject = Instantiate(_weaponProjectile, transform.position, Quaternion.identity);
        newGameObject.layer = layerToSet;
        LinearProjectile projectile = newGameObject.GetComponent<LinearProjectile>();
        bool halfStartSpeed = _playerWeapon == false && (projectile._startSpeed > 8 && _shooterStats._projectilesPerShot == 1);
        SetProjectileUp(projectile, halfStartSpeed);

        for (int i = 1; i < amountToAdd; i++)
        {
            newGameObject = Instantiate(_weaponProjectile, transform.position, Quaternion.identity);
            newGameObject.layer = layerToSet;
            projectile = newGameObject.GetComponent<LinearProjectile>();
            SetProjectileUp(projectile, halfStartSpeed);
        }
    }


    private void SetProjectileUp(LinearProjectile projectile, bool halfStartSpeed)
    {
        projectile._returnToPool = HandleReturnToPool;
        projectile._weaponDamage = _shooterStats._weaponDamage;
        _projectilesNotInUse.Push(projectile);

        projectile._startSpeed = halfStartSpeed ? projectile._startSpeed / 2f : projectile._startSpeed;
        projectile.gameObject.SetActive(false);
    }
}
