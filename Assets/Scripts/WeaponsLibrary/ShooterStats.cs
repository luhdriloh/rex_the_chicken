using UnityEngine;

[CreateAssetMenu(fileName = "ShooterState.asset", menuName = "Shooter/Shooter Stats", order = 1)]
public class ShooterStats : ScriptableObject
{
    public string _description;

    public float _recoilInDegrees;

    public int _weaponDamage;

    // magazine size and bool to indicate if it is loaded one by one
    public int _ammoCapacity;
    public int _magazineSize;
    public bool _usesMagazine;
    public float _reloadTime;

    public int _rpmMax;

    public int _rpmTapMax;

    public int _projectilesPerShot;
}