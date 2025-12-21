using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyShopUI : MonoBehaviour
{
    public TMP_Text goldText;
    public int pistolCost = 3;
    public int ammoPackAmount = 10;
    public int ammoPackCost = 5;
    public int healCost = 8;

    public string nextLevelScene = "Level1";

    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;

    // 1. Add this reference at the top of LobbyShopUI.cs
    public LoadoutManager loadoutManager; 

    public void BuyPistol()
    {
        if (GameSession.Instance == null) return;
        
        // Check if we already own it to prevent double-buying
        if (GameSession.Instance.ownedWeapons.Contains(WeaponId.Pistol)) return;

        if (GameSession.Instance.gold >= pistolCost)
        {
            // 1. Spend the gold
            GameSession.Instance.gold -= pistolCost;
            
            // 2. Add to the persistent "Owned" list
            GameSession.Instance.ownedWeapons.Add(WeaponId.Pistol);

            // 3. TELL THE LOADOUT MANAGER TO UPDATE THE DROPDOWNS
            if (loadoutManager != null) 
            {
                loadoutManager.RefreshDropdowns();
            }

            // 4. Update the Gold Text display
            RefreshUI();
        }
    }

    void OnEnable()
    {
        RefreshUI();
    }

    void Start()
    {
        // Optional: only if you actually have a player IN the lobby.
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerCombat = FindObjectOfType<PlayerCombat>();

        RefreshUI();
    }

    void RefreshUI()
    {
        if (goldText && GameSession.Instance != null)
            goldText.text = GameSession.Instance.gold.ToString();
    }


    public void ClaimStarterPistol()
    {
        if (GameSession.Instance == null) return;

        GameSession.Instance.ownedWeapons.Add(WeaponId.Pistol);

        // Only runs if you have a lobby player
        if (playerCombat != null) playerCombat.SetOwned(WeaponId.Pistol, true);

        RefreshUI();
    }

    public void BuyAmmoForCurrentWeapon()
    {
        if (GameSession.Instance == null) return;
        if (playerCombat == null) return;

        if (GameSession.Instance.gold < ammoPackCost) return;

        var current = playerCombat.weapons[playerCombat.currentWeaponIndex];
        if (current.data.infiniteAmmo) return;

        GameSession.Instance.gold -= ammoPackCost;
        playerCombat.AddAmmo(current.data.id, ammoPackAmount);

        RefreshUI();
    }

    public void HealOne()
    {
        if (GameSession.Instance == null) return;
        if (playerHealth == null) return;

        if (playerHealth.hp >= playerHealth.maxHP) return;
        if (GameSession.Instance.gold < healCost) return;

        GameSession.Instance.gold -= healCost;
        playerHealth.Heal(1);

        RefreshUI();
    }

    public void GoToNextLevel()
    {
        SceneManager.LoadScene(nextLevelScene);
    }


}
