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
}