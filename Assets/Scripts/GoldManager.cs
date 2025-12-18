using UnityEngine;

public static class GoldManager
{
    const string Key = "GOLD";

    public static int Gold
    {
        get => PlayerPrefs.GetInt(Key, 0);
        set { PlayerPrefs.SetInt(Key, value); PlayerPrefs.Save(); }
    }

    public static void Add(int amount) => Gold = Gold + amount;
    public static bool Spend(int amount)
    {
        if (Gold < amount) return false;
        Gold = Gold - amount;
        return true;
    }
}
