using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraMover : MonoBehaviour
{
    public GameObject targetToFollow;

    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 rotationOffset;
    public float distanceToTarget;
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
        targetToFollow = playerController.gameObject;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraStateMachine();
    }

    void CameraStateMachine()
    {
        switch (cameraState)
        {
            case ECameraState.Follow:
                FollowTarget();
                break;
            case ECameraState.Stand:
                break;
        }
    }

   
    void FollowTarget()
    {
        targetMoveDir = targetToFollow.transform.position - targetPrevPos;
        if (targetMoveDir != Vector3.zero)
        {
            targetMoveDir.Normalize();
            Vector3 targetPos = targetToFollow.transform.position - targetMoveDir * distanceToTarget;
            targetPos += offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
            targetPrevPos = targetToFollow.transform.position;
        }

        transform.rotation = Quaternion.LookRotation(targetToFollow.transform.forward + rotationOffset, Vector3.up);
      
    }
}
