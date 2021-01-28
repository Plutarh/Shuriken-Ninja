using BzKovSoft.CharacterSlicerSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Health))]
public class Pawn : MonoBehaviour , IDamageable
{

    public Animator animator;
    public Rigidbody body;
    public NavMeshAgent navMeshAgent;
    public Health health;
    public CharacterSlicerSampleFast characterSlicer;

    void Awake()
    {

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Init()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!body) body = GetComponent<Rigidbody>();
        if (!navMeshAgent) navMeshAgent = GetComponent<NavMeshAgent>();
        if (!characterSlicer) characterSlicer = GetComponent<CharacterSlicerSampleFast>();
    }

    public void Death()
    {
        
    }

    public void TakeDamage(float dmg)
    {
        
    }
}
