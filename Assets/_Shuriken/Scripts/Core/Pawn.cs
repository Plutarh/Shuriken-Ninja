﻿using BzKovSoft.CharacterSlicerSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BzKovSoft.CharacterSlicer;
using BzKovSoft.ObjectSlicerSamples;

[RequireComponent(typeof(Health))]
public class Pawn : MonoBehaviour , IDamageable
{

    public Animator animator;
    public Rigidbody body;
    public NavMeshAgent navMeshAgent;
    public Health health;
    public CharacterSlicerSampleFast characterSlicer;
    public AdderSliceableAsync adderSliceable;

    public bool dead;
    public EPawnType pawnType;
    public enum EPawnType
    {
        Player,
        Enemy
    }

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
        if (!adderSliceable) adderSliceable = GetComponent<AdderSliceableAsync>();
    }

    public void Death()
    {
        
    }

    public virtual void TakeDamage(float dmg)
    {
        
    }
}
