using UnityEngine;

public enum MagazineType
{
    Pistol,
    Rifle
}

public class Magazine : MonoBehaviour
{
    [SerializeField] private MagazineType magazineType;
    public int MaxAmmo;
    public int currentAmmo;
    void Start()
    {
        currentAmmo = MaxAmmo;
    }
    
    public void consumeAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
        }
    }
}
