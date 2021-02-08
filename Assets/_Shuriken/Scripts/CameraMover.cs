using System.Collections;
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
    public float rotateSpeed;

    public ECameraState cameraState;

    public enum ECameraState
    {
        Follow,
        Stand
    }


    Vector3 targetPrevPos;
    Vector3 targetMoveDir;


    [Inject]
    void Constuct(PlayerController playerController)
    {
        targetToFollow = playerController;
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
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetToFollow.transform.forward + followRotationOffset, Vector3.up), Time.deltaTime * 5);
    }

    void StandNearTarget()
    {
        Vector3 targetPos = targetToFollow.transform.position - targetMoveDir * standDistToTarget;
        targetPos += standOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetToFollow.transform.forward + standRotationOffset, Vector3.up), Time.deltaTime * 5);
    }
}
