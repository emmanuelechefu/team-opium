using UnityEngine;

public enum WeaponId { Rocks, Pistol /* add more later */ }

[CreateAssetMenu(menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponId id;
    public string displayName;

    public Sprite weaponIcon; // Drag your 'Pistol' or 'Rock' png here in the Inspector
    
    [Header("Visuals - Weapon Flash")]
    public Sprite weaponFlashSprite;    
    public float weaponFlashScale = 1f;
    public float weaponFlashOffset = 0.3f;

    [Header("Visuals - Muzzle Flash")]
    public bool enableMuzzleFlash;      
    public float muzzleFlashScale = 1f;
    public float muzzleFlashOffset = 1.0f;

    [Header("Combat")]
    public Projectile projectilePrefab;
    public float fireCooldown = 0.25f;
    public int damage = 1;
    public float projectileSpeed = 12f;

    [Header("Ammo")]
    public bool infiniteAmmo = false;
    public int startAmmo = 30;
    public int maxAmmo = 30;

    [Tooltip("For Rocks: regen 1 ammo every X seconds.")]
    public bool regenAmmoOverTime = false;
    public float regenInterval = 2.5f;

    [Header("Collision")]
    public bool projectilePassesThroughWalls = false;
}