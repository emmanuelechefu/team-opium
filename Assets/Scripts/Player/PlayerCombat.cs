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
        weapons.Clear();

        // 1. Identify which weapons were equipped in the lobby
        WeaponId slot1 = (GameSession.Instance != null) ? GameSession.Instance.equippedSlot1 : WeaponId.Rocks;
        WeaponId slot2 = (GameSession.Instance != null) ? GameSession.Instance.equippedSlot2 : WeaponId.Rocks;

        // 2. Add Slot 1
        AddWeaponByInstance(slot1);

        // 3. Add Slot 2 (only if it's different to avoid duplicates in the runtime list)
        if (slot1 != slot2)
        {
            AddWeaponByInstance(slot2);
        }

        currentWeaponIndex = 0;
    }

    private void AddWeaponByInstance(WeaponId id)
    {
        // Safety check: Only equip if the player actually owns it in the GameSession
        if (GameSession.Instance != null && !GameSession.Instance.ownedWeapons.Contains(id))
        {
            Debug.LogWarning($"Player tried to equip {id} but doesn't own it! Defaulting to Rocks.");
            id = WeaponId.Rocks; 
        }

        foreach (var wd in startingWeaponDatas)
        {
            if (wd != null && wd.id == id)
            {
                // Ensure we don't instantiate a null prefab
                if (wd.projectilePrefab == null)
                {
                    Debug.LogError($"Weapon {id} is missing its Projectile Prefab!");
                    return;
                }

                var wi = new WeaponInstance { data = wd, owned = true };
                wi.Init();
                weapons.Add(wi);
                return;
            }
        }
    }

    // Helper method to find the correct Data asset and create a Runtime instance
    private void AddWeaponToRuntimeList(WeaponId id)
    {
        foreach (var wd in startingWeaponDatas)
        {
            if (wd.id == id)
            {
                var wi = new WeaponInstance { data = wd, owned = true };
                wi.Init();
                weapons.Add(wi);
                return;
            }
        }
        Debug.LogError($"WeaponData for {id} not found in PlayerCombat's startingWeaponDatas list!");
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
