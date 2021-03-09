using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapActivator : MonoBehaviour
{

    public UnityEvent OnActivate;
    public bool activated;

    public Collider triggerCollider;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;



        if (!activated)
        {
            OnActivate?.Invoke();
            activated = true;
            triggerCollider.enabled = false;
        }
     
    }
}
