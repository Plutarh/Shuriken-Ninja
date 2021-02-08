﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraMover : MonoBehaviour
{
    public PlayerController targetToFollow;

    [SerializeField] Vector3 followOffset;
    [SerializeField] Vector3 followRotationOffset;

    [SerializeField] Vector3 standOffset;
    [SerializeField] Vector3 standRotationOffset;
    public float followDistToTarget;
    public float standDistToTarget;
    public float followSpeed;
    public float followRotateSpeed;
    public float standRotateSpeed;

    public ECameraState cameraState;

    public enum ECameraState
    {
        Follow,
        Stand
    }


    Vector3 targetPrevPos;
    Vector3 targetMoveDir;


    LevelSessionService levelSession;

    [Inject]
    void Constuct(PlayerController playerController, LevelSessionService lvlSession)
    {
        targetToFollow = playerController;
        levelSession = lvlSession;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerState();
        FindClosestEnemy();
    }

    private void LateUpdate()
    {
        CameraStateMachine();
    }

    void CheckPlayerState()
    {
        switch (targetToFollow.playerState)
        {
            case PlayerController.EPlayerState.MoveToPoint:
                ChangeState(ECameraState.Follow);
                break;
            case PlayerController.EPlayerState.Stand:
                ChangeState(ECameraState.Stand);
                break;
        }
    }

    void ChangeState(ECameraState newState)
    {
        if (cameraState == newState) return;
        cameraState = newState;
    }

    void CameraStateMachine()
    {
        switch (cameraState)
        {
            case ECameraState.Follow:
                FollowTarget();
                break;
            case ECameraState.Stand:
                StandNearTarget();
                break;
        }

        
    }

   
    void FollowTarget()
    {
        targetMoveDir = targetToFollow.transform.position - targetPrevPos;
        if (targetMoveDir != Vector3.zero)
        {
            targetMoveDir.Normalize();
            Vector3 targetPos = targetToFollow.transform.position - targetMoveDir * followDistToTarget;
            targetPos += followOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
            targetPrevPos = targetToFollow.transform.position;

            Debug.DrawLine(transform.position, targetPos, Color.red, 0.1f);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetToFollow.transform.forward + followRotationOffset, Vector3.up), Time.deltaTime * 5);
    }

    void StandNearTarget()
    {
       

        if(closestEnemy != null)
        {
            Vector3 targetPos = closestEnemy.transform.position - targetToFollow.transform.position;
            Vector3 dir = closestEnemy.transform.position - targetToFollow.transform.position;

           

            Debug.DrawRay(targetToFollow.transform.position, dir, Color.green);
            Debug.DrawRay(targetToFollow.transform.position, -dir.normalized, Color.black);
            Vector3 relativePos = targetToFollow.transform.position + (-dir.normalized) * standDistToTarget;
            Debug.DrawLine(targetToFollow.transform.position, relativePos, Color.red, 0.1f);

            //targetPos += standOffset;
            transform.position = Vector3.Lerp(transform.position, relativePos + standOffset, Time.deltaTime * followSpeed);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(closestEnemy.transform.position - targetToFollow.transform.position, Vector3.up), Time.deltaTime * standRotateSpeed);
            targetToFollow.transform.rotation = Quaternion.Slerp(targetToFollow.transform.rotation, Quaternion.LookRotation(targetPos, Vector3.up), Time.deltaTime * standRotateSpeed);
        }
        else
        {
            Vector3 targetPos = targetToFollow.transform.position - targetMoveDir * standDistToTarget;
            targetPos += standOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
           
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetToFollow.transform.forward + standRotationOffset, Vector3.up), Time.deltaTime * standRotateSpeed);
        
       
    }

    public GameObject closestEnemy;
    public float closestDist;

    void FindClosestEnemy()
    {
        if (levelSession == null)
        {
            Debug.LogError("Level Session for Camera is NULL", this);
            return;
        }

        closestEnemy = null;

        foreach (var enemy in levelSession.currentActionPoint.actionPointEnemies)
        {
            if (enemy == null) continue;
            float dist = (enemy.transform.position - targetToFollow.transform.position).sqrMagnitude;
            if(closestEnemy == null || dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemy.gameObject;
            }
        }
    }
}
