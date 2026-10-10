using UnityEngine;

public interface IDamageReceiver
{
    void OnDamaged(float damage, Vector2 knockBack);
}