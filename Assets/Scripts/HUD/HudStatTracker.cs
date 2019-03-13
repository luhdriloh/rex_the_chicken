using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class HudStatTracker : MonoBehaviour
{
    public Text _healthLeft;
    public Text _ammoLeft;

    private void Awake()
    {
        // subscribe to player health change event
        Player player = GameObject.Find("Rex").GetComponent<Player>();
        player.AddHealthChangeSubscriber(HandlePlayerHealthChange);

        // subscribe to ammo change events
        PlayerWeapon.AddAmmoChangeHandler(HandleAmmoChange);
    }


    private void HandlePlayerHealthChange(int newHealthValue, int maxHealthValue)
    {
        _healthLeft.text = "Health   " + newHealthValue + " / " + maxHealthValue;
    }


    private void HandleAmmoChange(Dictionary<WeaponType, int> ammoLeft)
    {
        //_ammoLeft.text = "Ammo    " + ammoLeft;
    }
}
