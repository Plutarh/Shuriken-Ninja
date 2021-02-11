using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public int ID;
    public float damage;
    public float durability;

    //TODO sound effects
    [Header("FX")]
    public ParticleSystem hitImpact;
}