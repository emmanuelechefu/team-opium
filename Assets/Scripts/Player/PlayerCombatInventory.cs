using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerCombatInventory : MonoBehaviour
{
    public Transform firePoint;
    public Camera mainCam;

    private Inventory inv;
    private float nextFireTime;
    private float regenTimer;

    void Awake()
    {
        inv = GetComponent<Inventory>();
        if (mainCam == null) mainCam = Camera.main;
    }

    void Update()
    {
        if (firePoint == null || mainCam == null || inv.weapons.Count == 0) return;

        HandleScroll();
        HandleRocksRegen();

        if (Input.GetKeyDown(KeyCode.Space))
            TryFire();

        inv.FallbackToRocksIfEmpty();
    }

    void HandleScroll()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < 0.01f) return;
        inv.TrySetCurrentByScroll(scroll > 0 ? 1 : -1);
    }

    void TryFire()
    {
        var w = inv.Current;
        if (Time.time < nextFireTime) return;

        if (!w.data.infiniteAmmo && w.ammo <= 0) return;

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - firePoint.position);
        dir.Normalize();

        var p = Instantiate(w.data.projectilePrefab, firePoint.position, Quaternion.identity);
        p.Init(dir, w.data.projectileSpeed, w.data.damage, w.data.projectilePassesThroughWalls, ownerTag: "Player");

        nextFireTime = Time.time + w.data.fireCooldown;

        if (!w.data.infiniteAmmo)
            w.ammo = Mathf.Max(0, w.ammo - 1);
    }

    void HandleRocksRegen()
    {
        int rocksIndex = inv.FindIndex(WeaponId.Rocks);
        if (rocksIndex < 0) return;

        var rocks = inv.weapons[rocksIndex];
        if (!rocks.data.regenAmmoOverTime || rocks.data.infiniteAmmo) return;
        if (rocks.ammo >= rocks.data.maxAmmo) { regenTimer = 0f; return; }

        regenTimer += Time.deltaTime;
        if (regenTimer >= rocks.data.regenInterval)
        {
            regenTimer -= rocks.data.regenInterval;
            rocks.ammo = Mathf.Min(rocks.data.maxAmmo, rocks.ammo + 1);
        }
    }
}
