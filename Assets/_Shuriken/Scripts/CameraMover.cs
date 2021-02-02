using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraMover : MonoBehaviour
{
    public GameObject targetToFollow;

    [SerializeField] Vector3 offset;
    public float followSpeed;

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
        FollowTarget();
    }

    void FollowTarget()
    {
        transform.position = Vector3.Lerp(transform.position, targetToFollow.transform.position + offset, Time.deltaTime * followSpeed);
    }
}
