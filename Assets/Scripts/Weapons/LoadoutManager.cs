using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadoutManager : MonoBehaviour
{
    public TMP_Dropdown slot1Dropdown;
    public TMP_Dropdown slot2Dropdown;

    private List<WeaponId> ownedList = new();

    void OnEnable()
    {
        RefreshDropdowns();
    }

    public void RefreshDropdowns()
    {
        if (GameSession.Instance == null) return;

        // 1. Get current owned weapons
        ownedList = new List<WeaponId>(GameSession.Instance.ownedWeapons);

        // 2. Clear and fill options
        slot1Dropdown.ClearOptions();
        slot2Dropdown.ClearOptions();

        List<string> options = new();
        foreach (var id in ownedList) options.Add(id.ToString());

        slot1Dropdown.AddOptions(options);
        slot2Dropdown.AddOptions(options);
    }

    public void OnSlotChanged(int dummyIndex)
    {
        // Logic check: Prevent duplicate weapons
        if (slot1Dropdown.value == slot2Dropdown.value && ownedList.Count > 1)
        {
            Debug.LogWarning("Cannot equip the same weapon in both slots!");
            // Optional: Revert to previous value or force change
            return;
        }

        // Save selected weapons to GameSession (you'll need to add these fields to GameSession)
        GameSession.Instance.equippedSlot1 = ownedList[slot1Dropdown.value];
        GameSession.Instance.equippedSlot2 = ownedList[slot2Dropdown.value];
    }
}