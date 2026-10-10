using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private float invincibleTime;

    private float invincibleTimer;
    private IDamageReceiver receiver;
}