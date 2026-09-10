using System.Collections.Generic;
using UnityEngine;

/// TO USE:
/// - attatch to hitbox prefab
/// - hitbox prefab should have a capsule collider
public class AttackHitbox : MonoBehaviour
{
    private float _damage = 40;
    private readonly HashSet<Collider> _alreadyHit = new HashSet<Collider>();

    /// Call immediately after Instantiate to set damage for this swing.
    public void Init(float damage)
    {
        _damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_alreadyHit.Contains(other))
            return;

        if (!other.CompareTag("Enemy"))
            return;

        _alreadyHit.Add(other);

        // TODO: call your enemy damage interface here
        Debug.Log($"Hit "+ other.name + " for " + _damage + " damage");
        other.GetComponent<GolemBehavior>().TakeDamage(_damage);
    }
}