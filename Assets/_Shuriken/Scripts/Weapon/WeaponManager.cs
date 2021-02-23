using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    public WeaponData weaponData;

    public PlayerController playerInstance;

    void Start()
    {
        //Debug.unityLogger.logEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Weapon GetWeaponByName(string weaponName)
    {
        return weaponData.weaponPrefabs.FirstOrDefault(wp => wp.name == weaponName);
    }

    public void ChangeWeapon()
    {
        if (playerInstance == null) return;

        /*
        if(playerInstance.weaponPrefab.weaponName == "Kunai")
            playerInstance.weaponPrefab = GetWeaponByName("Shuriken");
        else
            playerInstance.weaponPrefab = GetWeaponByName("Kunai");*/
    }
    
}
