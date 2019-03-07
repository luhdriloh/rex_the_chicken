using UnityEngine;

public interface IDamager
{
    void TakeDamage(int amountOfDamage, Vector2 bulletDirection);
}
