using UnityEngine;

public class Enemy2 : EnemyBase
{
    public float followSpeed = 3f;
    public float hoverHeight = 3f;

    protected override void Start()
    {
        base.Start();
        // ghost should pass through everything:
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 target = player.position + Vector3.up * hoverHeight;
        transform.position = Vector3.MoveTowards(transform.position, target, followSpeed * Time.deltaTime);
    }

    // still damages player when overlapping
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerHealth>(out var ph))
            ph.TakeDamage(touchDamage);
    }
}
