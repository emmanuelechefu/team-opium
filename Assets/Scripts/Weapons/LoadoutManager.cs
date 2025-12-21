using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LoadoutManager : MonoBehaviour
{
    [Header("UI Folders")]
    public TMP_Dropdown dropdown1; // Drag 'Dropdown 1' folder here
    public TMP_Dropdown dropdown2; // Drag 'Dropdown 2' folder here

    void Start()
    {
        RefreshDropdowns();

        // These "Listeners" detect when the player clicks a new option
        dropdown1.onValueChanged.AddListener(delegate { SaveSelection(); });
        dropdown2.onValueChanged.AddListener(delegate { SaveSelection(); });
    }

    public void RefreshDropdowns()
    {
        if (GameSession.Instance == null) return;

        // 1. Get real weapon names from the HashSet in GameSession
        List<string> options = new List<string>();
        foreach (WeaponId id in GameSession.Instance.ownedWeapons)
        {
            options.Add(id.ToString());
        }

        // 2. Clear placeholder "Option A" and add "Rocks/Pistol"
        dropdown1.ClearOptions();
        dropdown2.ClearOptions();
        dropdown1.AddOptions(options);
        dropdown2.AddOptions(options);

        // 3. Set the dropdowns to show what is ALREADY equipped in GameSession
        dropdown1.value = options.FindIndex(x => x == GameSession.Instance.equippedSlot1.ToString());
        dropdown2.value = options.FindIndex(x => x == GameSession.Instance.equippedSlot2.ToString());
        
        dropdown1.RefreshShownValue();
        dropdown2.RefreshShownValue();
    }

    // This is the missing piece that "Saves" the choice
    public void SaveSelection()
    {
        if (GameSession.Instance == null) return;

        // Get the text currently visible in the boxes
        string choice1 = dropdown1.options[dropdown1.value].text;
        string choice2 = dropdown2.options[dropdown2.value].text;

        // Convert that text back into the Enum ID and save it to GameSession
        if (System.Enum.TryParse(choice1, out WeaponId id1)) 
            GameSession.Instance.equippedSlot1 = id1;

        if (System.Enum.TryParse(choice2, out WeaponId id2)) 
            GameSession.Instance.equippedSlot2 = id2;

        Debug.Log($"Loadout Saved to Session: Slot1={id1}, Slot2={id2}");
    }
}