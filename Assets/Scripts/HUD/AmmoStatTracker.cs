using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoStatTracker : MonoBehaviour
{
    public Text _lightAmmoText;
    public Text _specialAmmoText;
    public Text _heavyAmmoText;

    public GameObject _lightAmmoBar;
    public GameObject _specialAmmoBar;
    public GameObject _heavyAmmoBar;

    public Color _fullAmmoColor;
    public Color _emptyAmmoColor;

    public float _fullYBarPosition;
    public float _emptyYBarPosition;

    private SpriteRenderer _lightAmmoBarSprite;
    private SpriteRenderer _specialAmmoBarSprite;
    private SpriteRenderer _heavyAmmoBarSprite;

    private float _minMaxBarDifference;

    private Dictionary<WeaponType, int> _weaponAmmoMax;

    private void Start()
    {
        _weaponAmmoMax = PlayerWeapon.GetWeaponAmmoMax();
        PlayerWeapon.AddAmmoChangeHandler(HandleAmmoChange);

        _minMaxBarDifference = _fullYBarPosition - _emptyYBarPosition;

        _lightAmmoBarSprite = _lightAmmoBar.GetComponent<SpriteRenderer>();
        _specialAmmoBarSprite = _specialAmmoBar.GetComponent<SpriteRenderer>();
        _heavyAmmoBarSprite = _heavyAmmoBar.GetComponent<SpriteRenderer>();
    }

    private void HandleAmmoChange(Dictionary<WeaponType, int> ammoLeft)
    {
        float lightAmmoPercent = ammoLeft[WeaponType.LIGHT] / (float)_weaponAmmoMax[WeaponType.LIGHT];
        float specialAmmoPercent = ammoLeft[WeaponType.SPECIAL] / (float)_weaponAmmoMax[WeaponType.SPECIAL];
        float heavyAmmoPercent = ammoLeft[WeaponType.HEAVY] / (float)_weaponAmmoMax[WeaponType.HEAVY];

        // set ammo text
        _lightAmmoText.text = ammoLeft[WeaponType.LIGHT].ToString();
        _specialAmmoText.text = ammoLeft[WeaponType.SPECIAL].ToString();
        _heavyAmmoText.text = ammoLeft[WeaponType.HEAVY].ToString();

        // set bars
        _lightAmmoBar.transform.localPosition = new Vector2(_lightAmmoBar.transform.localPosition.x, _emptyYBarPosition + lightAmmoPercent * _minMaxBarDifference);
        _specialAmmoBar.transform.localPosition = new Vector2(_specialAmmoBar.transform.localPosition.x, _emptyYBarPosition + specialAmmoPercent * _minMaxBarDifference);
        _heavyAmmoBar.transform.localPosition = new Vector2(_heavyAmmoBar.transform.localPosition.x, _emptyYBarPosition + heavyAmmoPercent * _minMaxBarDifference);

        // set color up
        _lightAmmoBarSprite.color = Color.Lerp(_emptyAmmoColor, _fullAmmoColor, lightAmmoPercent);
        _specialAmmoBarSprite.color = Color.Lerp(_emptyAmmoColor, _fullAmmoColor, specialAmmoPercent);
        _heavyAmmoBarSprite.color = Color.Lerp(_emptyAmmoColor, _fullAmmoColor, heavyAmmoPercent);
    }
}
