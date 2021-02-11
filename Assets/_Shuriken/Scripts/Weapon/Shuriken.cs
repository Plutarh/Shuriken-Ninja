﻿using BzKovSoft.ObjectSlicerSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Shuriken : Weapon , IThrowable
{

    [SerializeField] BzKnife slicer;


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

    public EMoveType moveType;

    void Awake()
    {
        
        slicer.OnSliceBegin += BeginSlice;
        slicer.OnStopSlice += StopSlice;
    }
    
    void Start()
    {
        bezieMove = true;
        SetRandomYaw();

        slicer.SliceID = Random.Range(-100, 100);
    }

    
    void Update()
    {
        Movement();
        Rotation();
    }

    void BeginSlice()
    {
       
    }

    void StopSlice()
    {
        moveSpeed = 0;
        mainRotateSpeed = 0;
        secondRotateSpeed = 0;
        trail.emitting = false;
        HitImpact();
    }

    void HitImpact()
    {
        if (hitImpact != null)
        {
            if (!hitImpact.isPlaying)
                hitImpact.Play();
        }

      
    }

    void SetRandomYaw()
    {
        mainRotateDir.z = Random.Range(-1.9f, 2);
    }

    public void SetMoveType(EMoveType type)
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

    void Movement()
    {
        switch (moveType)
        {
            case EMoveType.Free:
               
                MoveToDirection();
                break;
            case EMoveType.Target:
                MoveToDirection();
                //MoveByBezieCurve();
                break;
        }
    }

    void MoveByBezieCurve()
    {
        tParam += Time.deltaTime * moveSpeed;
        if (tParam < 1)
        {
            transform.position = BezieCurve.GetPointOnBezierCurve
             (startPos
             , startAnchor
             , endAnchor
             , endPos
             , tParam);
        }
        else gameObject.SetActive(false);
    }

    void MoveToDirection()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    void Rotation()
    {
        if (!slicer.sliceable) return;
        /*
        Quaternion yaw = Quaternion.Euler(mainRotateDir * Time.deltaTime * mainRotateSpeed);
        mainRotateObject.transform.localRotation = yaw * mainRotateObject.transform.localRotation;
        */

        //Quaternion pitch = Quaternion.Euler(secondaryRotateDir * Time.deltaTime * secondRotateSpeed);
        //mainRotateObject.transform.rotation = mainRotateObject.transform.rotation * pitch;

        
        Quaternion pitch = Quaternion.Euler(secondaryRotateDir * Time.deltaTime * secondRotateSpeed);
        secondaryRotateObject.transform.rotation = secondaryRotateObject.transform.rotation * pitch;
        
        
    }

    void OnDestroy()
    {
        slicer.OnSliceBegin -= BeginSlice;
        slicer.OnStopSlice -= StopSlice;
    }
}
