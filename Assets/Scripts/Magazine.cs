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
    public BaseGun basegun;
    public MagazineEject mageject;
    void Start()
    {
        currentAmmo = MaxAmmo;
    }

    void Update()
    {
        if (basegun == null)
        {
            basegun = GetComponentInParent<BaseGun>();
        }
        if (mageject == null)
        {
            mageject = GetComponentInParent<MagazineEject>();
        }
    }

    public void EjectMag()
    {
        basegun.magazine = null;
        mageject.mag = null;
    }
    
    public void consumeAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
        }
    }
}
