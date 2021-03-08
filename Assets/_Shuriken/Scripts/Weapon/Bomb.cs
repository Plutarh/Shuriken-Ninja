using BzKovSoft.ObjectSlicerSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Weapon, IThrowable
{
    public EBombType bombType;
    public enum EBombType
    {
        Explosion,
        Ice,
        Poison
    }

    public float explosionRadius;

    public Color gizmoColor;

    public LayerMask layerMask;

    //public ParticleSystem explosionParticle;

    public List<AIEnemy> hittedPawns = new List<AIEnemy>();
    public StatusEffect statusEffect;

    
    public Shuriken.EMoveType moveType;

    [Header("Movement")]
    public Vector3 moveDir;
    public float moveSpeed;

    Vector3 startPos;
    Vector3 endPos;

    Vector3 startAnchor;
    Vector3 endAnchor;

    [Header("Rotation")]
    public Vector3 mainRotateDir;
    public Vector3 secondaryRotateDir;

    public float mainRotateSpeed;
    public float secondRotateSpeed;

    public GameObject mainRotateObject;
    public GameObject secondaryRotateObject;

    [Header("Distance")]
    public float vectorLength;

    [Header("Effects")]
    public TrailRenderer trail;

    float tParam;
    bool bezieMove;


    public GameObject target;
    Vector3 targetPoint;
    Collider targetCollider;

    bool trapFly;

    bool exploited;

    void Start()
    {
        
    }

    
    void Update()
    {
        Movement();
        Rotation();
    }

    void Movement()
    {
        switch (moveType)
        {
            case Shuriken.EMoveType.Free:
                MoveToDirection();
                break;
            case Shuriken.EMoveType.Target:
                MoveToTarget();
                break;
        }
    }

    void MoveToDirection()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    void MoveToTarget()
    {
        if (target == null) return;

        Vector3 moveDirToTarget;
        if (trapFly)
        {
            targetPoint = targetCollider.bounds.center;
            moveDirToTarget = targetPoint - transform.position;
        }
        else
        {
            moveDirToTarget = target.transform.position - transform.position;
        }

        // просто позиция обьекта
        //

        //moveDir =
        moveDirToTarget.Normalize();
        transform.Translate(moveDirToTarget * moveSpeed * Time.deltaTime);

    }

    void Rotation()
    {
       
        /*
        Quaternion yaw = Quaternion.Euler(mainRotateDir * Time.deltaTime * mainRotateSpeed);
        mainRotateObject.transform.localRotation = yaw * mainRotateObject.transform.localRotation;
        */

        //Quaternion pitch = Quaternion.Euler(secondaryRotateDir * Time.deltaTime * secondRotateSpeed);
        //mainRotateObject.transform.rotation = mainRotateObject.transform.rotation * pitch;


        Quaternion pitch = Quaternion.Euler(secondaryRotateDir * Time.deltaTime * secondRotateSpeed);
        secondaryRotateObject.transform.rotation = secondaryRotateObject.transform.rotation * pitch;


    }



    public bool IsSlicer()
    {
        return false;
    }

    public void SetMoveType(Shuriken.EMoveType type)
    {
        moveType = type;
    }

    public void SetTargetPosition(Vector3 tPos)
    {
        endPos = tPos;

        startPos = transform.position;
        vectorLength = (endPos - startPos).magnitude;

        startAnchor = startPos;
        endAnchor = endPos;

        moveDir = tPos - transform.position;
        moveDir.Normalize();
        secondaryRotateObject.transform.rotation = Quaternion.LookRotation(moveDir);
    }

    public void SetMoveDirection(Vector3 _dir)
    {


    }

    public void SetStartPosition(Vector3 _startPos)
    {

    }




    public void SetTargetCollider(Collider tCol)
    {
        target = tCol.gameObject;

        if (tCol.GetComponent<Trap>() != null)
        {
            trapFly = true;
            targetPoint = tCol.bounds.center;
            targetCollider = tCol;
        }
    }

    void Explosion()
    {
        if (hitImpact != null)
        {
            var explosion = Instantiate(hitImpact, transform);
            explosion.transform.SetParent(null);
            explosion.transform.position = this.transform.position;
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
            switch (bombType)
            {
                case EBombType.Explosion:
                    pawn.TakeDamage(damage, Vector3.zero, EDamageType.Explosion);
                    break;
                case EBombType.Ice:
                    pawn.TakeStatus(EStatusEffect.Freeze);
                    break;
                case EBombType.Poison:
                    pawn.TakeStatus(EStatusEffect.Poison);
                    break;
            }

        }
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other == null || exploited) return;

      
        var pawnKsa = other.GetComponent<KnifeSliceableAsync>();

        if(pawnKsa != null && pawnKsa.owner != null && pawnKsa.owner.pawnType != Pawn.EPawnType.Player)
        {
            Explosion();
            exploited = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawSphere(transform.position, explosionRadius);
    }

}
