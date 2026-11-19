using UnityEngine;

[System.Serializable]
public class AttackData
{
    [Min(0f)] public float damage;
    public Vector2 knockBack;
    [Min(0f)] public float invincibleTime;
}
