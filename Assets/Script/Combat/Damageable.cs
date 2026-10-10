using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField, Min(0f)] private float invincibleTime;

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

    private void Update()
    {
        if (invincibleTimer > 0f)
        {
            invincibleTimer = Mathf.Max(0f, invincibleTimer - Time.deltaTime);
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