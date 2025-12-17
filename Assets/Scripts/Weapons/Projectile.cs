using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public float lifeTime = 4f;

    private Rigidbody2D rb;
    private int damage;
    private bool passThroughWalls;
    private string ownerTag;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void Init(Vector2 direction, float speed, int dmg, bool passesWalls, string ownerTag)
    {
        this.damage = dmg;
        this.passThroughWalls = passesWalls;
        this.ownerTag = ownerTag;

        rb.linearVelocity = direction * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ownerTag)) return;

        // Hit enemy/player health
        if (other.TryGetComponent<IDamageable>(out var dmgable))
        {
            dmgable.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Walls
        if (!passThroughWalls && (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                                 other.gameObject.layer == LayerMask.NameToLayer("Walls")))
        {
            Destroy(gameObject);
        }
    }
}

public interface IDamageable
{
    void TakeDamage(int amount);
}
