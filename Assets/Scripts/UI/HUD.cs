using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Text Elements")]
    public TMP_Text goldText;
    public TMP_Text hpText;
    public TMP_Text weaponText;
    public TMP_Text ammoText;

    [Header("Visual Elements")]
    public Image hpBarFill;       // Drag 'HealthBar_Fill' here
    public Image weaponIconSlot;  // Drag 'CurrentWeapon_Icon' here

    private PlayerHealth ph;
    private Inventory inv;

    void Start()
    {
        RefreshReferences();
    }

    void RefreshReferences()
    {
        // Try to find components via GameManager first
        if (GameManager.Instance != null)
        {
            ph = GameManager.Instance.PlayerHealth;
            inv = GameManager.Instance.PlayerInventory;
        }

        // Fallback search
        if (ph == null) ph = FindFirstObjectByType<PlayerHealth>();
        if (inv == null) inv = FindFirstObjectByType<Inventory>();
    }

    void Update()
    {
        // Safety check: if references are lost (e.g. scene change), find them again
        if (ph == null || inv == null) { RefreshReferences(); return; }

        // 1. GOLD
        if (goldText && GameSession.Instance != null)
            goldText.text = $"GOLD: {GameSession.Instance.gold}";

        // 2. HEALTH BAR & TEXT
        if (ph != null)
        {
            if (hpText) hpText.text = $"HP: {ph.hp}/{ph.maxHP}";
            
            if (hpBarFill)
            {
                // Ensure we don't divide by zero
                float currentHp = (float)ph.hp;
                float maxHp = (float)ph.maxHP;
                hpBarFill.fillAmount = (maxHp > 0) ? currentHp / maxHp : 0;
            }
        }

        // 3. WEAPON ICON & AMMO
        if (inv != null && inv.weapons.Count > 0)
        {
            // Use currentIndex from the Inventory script
            var currentWeapon = inv.weapons[inv.currentIndex];

            if (weaponText) weaponText.text = currentWeapon.data.displayName;

            if (weaponIconSlot)
            {
                // Assign sprite from WeaponData
                weaponIconSlot.sprite = currentWeapon.data.weaponIcon;
                
                // Ensure alpha is visible
                Color c = weaponIconSlot.color;
                c.a = (weaponIconSlot.sprite != null) ? 1f : 0f;
                weaponIconSlot.color = c;
            }

            if (ammoText)
            {
                if (currentWeapon.data.infiniteAmmo) ammoText.text = "Ammo: ∞";
                else ammoText.text = $"Ammo: {currentWeapon.ammo}/{currentWeapon.data.maxAmmo}";
            }
        }
    }
}