using UnityEngine;

public class Enemy3 : EnemyBase
{
    public Projectile bulletPrefab;
    public float bulletSpeed = 18f;
    public float fireInterval = 1.2f;
    public float desiredMinDistance = 10f;
    public float desiredMaxDistance = 16f;
    public int bulletDamage = 1;

    private float t;

    void Update()
    {
        if (player == null) return;

        // Maintain distance horizontally (simple)
        float dx = player.position.x - transform.position.x;
        float adx = Mathf.Abs(dx);

        if (adx < desiredMinDistance)
            transform.position += new Vector3(-Mathf.Sign(dx) * 2f * Time.deltaTime, 0f, 0f);
        else if (adx > desiredMaxDistance)
            transform.position += new Vector3(Mathf.Sign(dx) * 2f * Time.deltaTime, 0f, 0f);

        t += Time.deltaTime;
        if (t >= fireInterval)
        {
            t -= fireInterval;
            Fire();
        }
    }

    void Fire()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        var p = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        // passes through walls/floors:
        p.Init(dir, bulletSpeed, bulletDamage, passesWalls: true, ownerTag: "Enemy");
    }
}
