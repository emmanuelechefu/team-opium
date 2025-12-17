[System.Serializable]
public class WeaponInstance
{
    public WeaponData data;
    public bool owned;
    public int ammo;

    public void Init()
    {
        ammo = data.infiniteAmmo ? int.MaxValue : data.startAmmo;
    }

    public int AmmoClamped => data.infiniteAmmo ? int.MaxValue : ammo;
}
