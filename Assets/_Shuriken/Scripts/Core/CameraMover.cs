using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraMover : MonoBehaviour
{
    public PlayerController targetToFollow;

    [SerializeField] Vector3 idleOffset;

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
        Stand,
        Idle
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

    private void Awake()
    {
        InitCamera();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerState();
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
            case PlayerController.EPlayerState.Death:
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
            case ECameraState.Idle:
                break;
        }
    }

    void InitCamera()
    {
        transform.rotation = Quaternion.LookRotation(targetToFollow.transform.forward + followRotationOffset, Vector3.up);
        Vector3 targetPos = targetToFollow.transform.position + followOffset;
        transform.position = targetToFollow.transform.position + idleOffset;
                
    }

   
    void FollowTarget()
    {
        targetMoveDir = targetToFollow.transform.position - targetPrevPos;
        if (targetMoveDir != Vector3.zero)
        {
            targetMoveDir.Normalize();
            Vector3 targetPos = targetToFollow.transform.position - targetMoveDir * followDistToTarget;
            targetPos += followOffset;

            transform.position = Vector3.Lerp(transform.position
                ,targetPos
                ,Time.deltaTime * followSpeed);

            targetPrevPos = targetToFollow.transform.position;
            //Debug.DrawLine(transform.position, targetPos, Color.red, 0.1f);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation
            ,Quaternion.LookRotation(targetToFollow.transform.forward + followRotationOffset, Vector3.up)
            ,Time.deltaTime * 5);
    }

    void StandNearTarget()
    {
       

        if(targetToFollow.closestEnemy != null)
        {
          
            Vector3 targetPos = targetToFollow.closestEnemy.transform.position - targetToFollow.transform.position;
            Vector3 dir = targetToFollow.closestEnemy.transform.position - targetToFollow.transform.position;
            Vector3 relativePos = targetToFollow.transform.position + (-dir.normalized) * standDistToTarget;

            transform.position = Vector3.Lerp(transform.position
                ,relativePos + standOffset
                ,Time.deltaTime * followSpeed * 2);
        }
        else
        {
            /*
            Vector3 targetPos = targetToFollow.transform.position - targetMoveDir * standDistToTarget;
            targetPos += standOffset;
            transform.position = Vector3.Lerp(transform.position
                ,targetPos
                ,Time.deltaTime * followSpeed);
           */
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation
            ,Quaternion.LookRotation(targetToFollow.transform.forward + standRotationOffset, Vector3.up)
            ,Time.deltaTime * standRotateSpeed);
    }


}
