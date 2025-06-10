using System;
using UnityEngine;
using BNG;

public enum MagazineType
{
    pistol,
    rifle
}

public class Magazine : MonoBehaviour
{
    [SerializeField] private MagazineType magType;
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