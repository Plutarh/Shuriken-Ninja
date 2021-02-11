using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName ="Create Weapon Data", order = 51)]
public class WeaponData : ScriptableObject
{
    public List<Weapon> weaponPrefabs = new List<Weapon>();
}
