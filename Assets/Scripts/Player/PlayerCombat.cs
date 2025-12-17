using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Refs")]
    public Transform firePoint;
    public Camera mainCam;

    [Header("Weapons (assign ScriptableObjects)")]
    public List<WeaponData> startingWeaponDatas;

    [Header("Runtime")]
    public List<WeaponInstance> weapons = new();
    public int currentWeaponIndex = 0;

    private float nextFireTime;
    private float regenTimer;

    void Start()
    {
        // Build instances
        weapons.Clear();
        foreach (var wd in startingWeaponDatas)
        {
            var wi = new WeaponInstance { data = wd, owned = (wd.id == WeaponId.Rocks) };
            wi.Init();

            // Pistol starter is free and owned in lobby, but you can mark it owned here if you want for testing:
            // if (wd.id == WeaponId.Pistol) wi.owned = true;

            weapons.Add(wi);
        }

        // Ensure current is Rocks initially
        currentWeaponIndex = FindWeaponIndex(WeaponId.Rocks);
    }

    void Update()
    {
        HandleScrollWheel();

        // Shoot with Space
        if (Input.GetKey(KeyCode.Space))
            TryFire();

        HandleRocksRegen();
        AutoFallbackToRocksIfEmpty();
    }

    void HandleScrollWheel()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) < 0.01f) return;

        // Cycle owned weapons only
        int dir = scroll > 0 ? 1 : -1;
        int start = currentWeaponIndex;

        for (int i = 0; i < weapons.Count; i++)
        {
            currentWeaponIndex = (currentWeaponIndex + dir + weapons.Count) % weapons.Count;
            if (weapons[currentWeaponIndex].owned) return;
        }

        currentWeaponIndex = start;
    }

    void TryFire()
    {
        var w = weapons[currentWeaponIndex];
        if (Time.time < nextFireTime) return;

        if (!w.data.infiniteAmmo && w.ammo <= 0) return;

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - firePoint.position);
        dir.Normalize();

        Projectile p = Instantiate(w.data.projectilePrefab, firePoint.position, Quaternion.identity);
        p.Init(dir, w.data.projectileSpeed, w.data.damage, w.data.projectilePassesThroughWalls, ownerTag: "Player");

        nextFireTime = Time.time + w.data.fireCooldown;

        if (!w.data.infiniteAmmo)
            w.ammo = Mathf.Max(0, w.ammo - 1);
    }

    void HandleRocksRegen()
    {
        var rocksIndex = FindWeaponIndex(WeaponId.Rocks);
        if (rocksIndex < 0) return;

        var rocks = weapons[rocksIndex];
        if (!rocks.data.regenAmmoOverTime) return;
        if (rocks.data.infiniteAmmo) return;

        if (rocks.ammo >= rocks.data.maxAmmo) { regenTimer = 0f; return; }

        regenTimer += Time.deltaTime;
        if (regenTimer >= rocks.data.regenInterval)
        {
            regenTimer -= rocks.data.regenInterval;
            rocks.ammo = Mathf.Min(rocks.data.maxAmmo, rocks.ammo + 1);
        }
    }

    void AutoFallbackToRocksIfEmpty()
    {
        var w = weapons[currentWeaponIndex];
        if (w.data.id == WeaponId.Rocks) return;
        if (w.data.infiniteAmmo) return;

        if (w.ammo <= 0)
        {
            int rocksIndex = FindWeaponIndex(WeaponId.Rocks);
            if (rocksIndex >= 0) currentWeaponIndex = rocksIndex;
        }
    }

    public int FindWeaponIndex(WeaponId id)
    {
        for (int i = 0; i < weapons.Count; i++)
            if (weapons[i].data.id == id) return i;
        return -1;
    }

    // Called by lobby shop:
    public void SetOwned(WeaponId id, bool owned)
    {
        int idx = FindWeaponIndex(id);
        if (idx >= 0) weapons[idx].owned = owned;
    }

    public void AddAmmo(WeaponId id, int amount)
    {
        int idx = FindWeaponIndex(id);
        if (idx < 0) return;

        var w = weapons[idx];
        if (w.data.infiniteAmmo) return;
        w.ammo = Mathf.Clamp(w.ammo + amount, 0, w.data.maxAmmo);
    }
}
