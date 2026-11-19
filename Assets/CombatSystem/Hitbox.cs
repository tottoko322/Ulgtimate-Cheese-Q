using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    [SerializeField] private Damageable owner;
    [SerializeField] private AttackData attackData = new AttackData();
    [SerializeField] private LayerMask targetLayers = 0;

    private Collider2D hitCollider;
    private readonly List<Collider2D> overlaps = new List<Collider2D>();
    private readonly HashSet<Damageable> hitTargets = new HashSet<Damageable>();
    private bool isAttacking;
    private float damage;
    private float invincibleTime;
    private Vector2 knockBack;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        hitCollider = GetComponent<Collider2D>();
        hitCollider.isTrigger = true;
        hitCollider.enabled = false;
        if (owner == null)
        {
            owner = GetComponentInParent<Damageable>();
        }
    }

    // Call once when the attack's active window begins. Facing is -1 or 1.
    public void BeginAttack(int facing)
    {
        if (!isActiveAndEnabled || hitCollider == null)
        {
            return;
        }
        if (owner == null || !owner.isActiveAndEnabled || attackData == null || facing == 0)
        {
            Debug.LogWarning("攻撃の持ち主・設定・方向を確認してください", this);
            return;
        }

        damage = Mathf.Max(0f, attackData.damage);
        invincibleTime = Mathf.Max(0f, attackData.invincibleTime);
        knockBack = new Vector2(
            Mathf.Abs(attackData.knockBack.x) * (facing < 0 ? -1 : 1),
            attackData.knockBack.y);
        hitTargets.Clear();
        isAttacking = true;
        hitCollider.enabled = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
        if (hitCollider != null)
        {
            hitCollider.enabled = false;
        }
        overlaps.Clear();
        hitTargets.Clear();
    }

    private void FixedUpdate()
    {
        if (!isAttacking)
        {
            return;
        }
        if (owner == null || !owner.isActiveAndEnabled)
        {
            EndAttack();
            return;
        }

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayers);
        filter.useTriggers = true;
        overlaps.Clear();
        hitCollider.Overlap(filter, overlaps);

        foreach (Collider2D other in overlaps)
        {
            if (!isAttacking)
            {
                break;
            }
            if (other == null || !other.enabled || !other.gameObject.activeInHierarchy)
            {
                continue;
            }

            Hurtbox hurtbox = other.GetComponent<Hurtbox>();
            if (hurtbox == null || !hurtbox.IsReceiving)
            {
                continue;
            }

            Damageable target = hurtbox.Receiver;
            if (target == owner || !hitTargets.Add(target))
            {
                continue;
            }
            target.TakeDamage(damage, knockBack, invincibleTime);
        }
    }

    private void OnDisable()
    {
        EndAttack();
    }
}
