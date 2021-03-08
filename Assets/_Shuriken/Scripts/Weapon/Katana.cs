using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : Weapon
{
    // Start is called before the first frame update
    void Start()
    {

        foreach (var col in owner.allColliders)
        {
            Physics.IgnoreCollision(col, GetComponent<Collider>());
        }

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (owner == null) return;

        Debug.LogError("Trigger OK");

        var otherPawn = other.GetComponent<Pawn>();
        if(otherPawn != null)
        {
            Debug.LogError("PAWN OK");
            if (otherPawn.pawnType != owner.pawnType)
            {
                otherPawn.TakeDamage(damage, Vector3.zero, EDamageType.Hit);
                Debug.LogError("DMG OK");
            }
        }
    }
}
