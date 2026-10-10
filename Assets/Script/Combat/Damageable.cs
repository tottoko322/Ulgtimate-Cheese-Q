using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private float invincibleTime;

    private float invincibleTimer;
    private IDamageReceiver receiver;

    private void Awake()
    {
        receiver = GetComponent<IDamageReceiver>();

        if (receiver == null)
        {
            Debug.LogError("IDamageReceiverが見つかりません", this);
        }
    }

    public void TakeDamage(float damage, Vector2 knockBack)
    {
        if (receiver == null || invincibleTimer > 0f)
        {
            return;
        }

        receiver.OnDamaged(damage, knockBack);

        invincibleTimer = invincibleTime;
    }
    
}