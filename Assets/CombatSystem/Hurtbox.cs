using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private Damageable receiver;
    [SerializeField] private bool startsEnabled = true;
    private Collider2D hurtCollider;

    public Damageable Receiver => receiver;
    public bool IsReceiving => isActiveAndEnabled
        && hurtCollider != null && hurtCollider.enabled
        && receiver != null && receiver.isActiveAndEnabled;

    private void Awake()
    {
        hurtCollider = GetComponent<Collider2D>();
        hurtCollider.isTrigger = true;
        if (receiver == null)
        {
            receiver = GetComponentInParent<Damageable>();
        }
        if (receiver == null)
        {
            Debug.LogError("食らい判定の持ち主にDamageableを設定してください", this);
        }
    }

    private void OnEnable()
    {
        SetReceiving(startsEnabled);
    }

    // Body: always enabled. Extended limbs and weapons: enable during animation.
    public void SetReceiving(bool value)
    {
        if (hurtCollider != null)
        {
            hurtCollider.enabled = value && receiver != null;
        }
    }

    private void OnDisable()
    {
        if (hurtCollider != null)
        {
            hurtCollider.enabled = false;
        }
    }
}
