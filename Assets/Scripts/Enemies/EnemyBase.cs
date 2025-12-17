using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    public int maxHP = 1;
    public int hp = 1;
    public int touchDamage = 1;
    public int goldDrop = 1;

    public GoldPickup goldPrefab;

    protected Transform player;

    protected virtual void Start()
    {
        hp = maxHP;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public virtual void TakeDamage(int amount)
    {
        hp = Mathf.Max(0, hp - amount);
        if (hp <= 0) Die();
    }

    protected virtual void Die()
    {
        if (goldPrefab != null)
        {
            var g = Instantiate(goldPrefab, transform.position, Quaternion.identity);
            g.amount = goldDrop;
        }
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player") && col.collider.TryGetComponent<PlayerHealth>(out var ph))
        {
            ph.TakeDamage(touchDamage);
        }
    }
}
