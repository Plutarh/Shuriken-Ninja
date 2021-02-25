using BzKovSoft.ObjectSlicerSamples;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionPot : Trap
{

    public float explosionRadius;
    public float explosionDamage;

    public Color gizmoColor;

    public LayerMask layerMask;

    public ParticleSystem explosionParticle;

    public List<AIEnemy> hittedPawns = new List<AIEnemy>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Explosion()
    {
        if (explosionParticle != null)
        {
            var explosion = Instantiate(explosionParticle, transform);
            explosion.transform.SetParent(null);
            Destroy(explosion, 3);
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
       

        foreach (var col in hitColliders)
        {
            var pawn = col.transform.root.GetComponent<AIEnemy>();
            if (pawn != null)
            {
                if (!hittedPawns.Contains(pawn)) hittedPawns.Add(pawn);
            }
        }

        foreach (var pawn in hittedPawns)
        {
            pawn.TakeDamage(explosionDamage, pawn.transform.position - transform.position, EDamageType.Explosion);
        }
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        var hit = other.GetComponent<BzKnife>();

        if(hit != null && hit.weapon.owner.pawnType == Pawn.EPawnType.Player)
        {
            Explosion();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
