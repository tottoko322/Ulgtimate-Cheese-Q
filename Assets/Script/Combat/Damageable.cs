using UnityEngine;

public class Damageable : MonoBehaviour
{

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

    public void TakeDamage(float damage, Vector2 knockBack, float invincibleTime)
    {
        if (!isActiveAndEnabled || receiver == null || invincibleTimer > 0f)
        {
            return;
        }

        receiver.OnDamaged(damage, knockBack);

        invincibleTimer = Mathf.Max(0f, invincibleTime);
    }    
    
}