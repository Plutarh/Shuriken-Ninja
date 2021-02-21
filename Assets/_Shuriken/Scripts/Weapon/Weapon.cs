using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public int ID;
    public float damage;
    public float durability;
    public EWeaponType weaponType;
    public enum EWeaponType
    {
        Melee,
        Range
    }

    //TODO sound effects
    [Header("FX")]
    public ParticleSystem hitImpact;
    public ParticleSystem bloodImpact;
}