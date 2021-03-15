using BzKovSoft.ObjectSlicerSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class TrapActivator : MonoBehaviour
{

    public UnityEvent OnActivate;
    public bool activated;

    public Collider triggerCollider;

    public Animator animator;
    public Rigidbody parentRb;

    private void Awake()
    {
        if (triggerCollider == null) triggerCollider = GetComponent<Collider>();
    }

    void Start()
    {
        
    }

   
    void Action()
    {
        OnActivate?.Invoke();
        activated = true;
        triggerCollider.isTrigger = false;
        //parentRb.isKinematic = false;
        //parentRb.useGravity = true;
        //animator.SetTrigger("Action");
        parentRb.transform.DOMoveY(parentRb.transform.position.y + 10, 7).SetEase(Ease.Linear).OnComplete(() =>
        {
            Destroy(parentRb.gameObject);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        var bzKnife = other.GetComponent<BzKnife>();

        if(bzKnife != null && bzKnife.weapon.owner.pawnType == Pawn.EPawnType.Player)
        {
            if (!activated)
            {
                Action();
                bzKnife.weapon.gameObject.AddComponent<Rigidbody>();
                //bzKnife.weapon.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }
}
