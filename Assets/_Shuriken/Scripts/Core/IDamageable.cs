using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float dmg, Vector3 dir, EDamageType damageType);
    void Death();

  
}

public enum EDamageType
{
    Hit,
    Explosion
}