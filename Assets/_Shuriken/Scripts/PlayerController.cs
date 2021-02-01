using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerController : Pawn
{
    public Vector3 shurikenOffset;
    public SimpleMover shurikenPrefab;

    public Transform R_shurikenSpawnPos;
    public Transform L_shurikenSpawnPos;

    IInputService _inputService;
    [Inject]
    void Construct(IInputService inputService)
    {
        _inputService = inputService;
        _inputService.OnColliderClick += GetThrowDirection;
    }

    private void OnValidate()
    {
        base.Init();
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
    public Vector3 throwDirection;

    public Vector3 relPoint;

    /*
    void GetThrowDirection(Vector3 point)
    {
        throwDir = point;
        // throwDir = point - (shurikenSpawnPos.position);
        Vector3 crossRes = Vector3.Cross(transform.position, point);
        direction = Vector3.Dot(crossRes, Vector3.up);

        if(direction > 0)
        {
            animator.SetTrigger("ThrowR");
        }
        else if(direction < 0){
            animator.SetTrigger("ThrowL");
        }
        else
        {
            animator.SetTrigger("ThrowR");
        }
       
    }*/

    void GetThrowDirection(Vector3 point,GameObject go)
    {
        throwDir = point;

        relPoint = transform.InverseTransformPoint(point);

        Debug.Log($"{relPoint.x} THROW");

        if (relPoint.x > 1)
            animator.SetTrigger("ThrowL");
        else if (relPoint.x < -1)
            animator.SetTrigger("ThrowR");
        else
            animator.SetTrigger("ThrowM");

    }

    public Vector3 throwDir;

    public void ThrowShurikenByAnimator()
    {
        ThrowShuriken(relPoint);
    }

    void ThrowShuriken(Vector3 relPoint)
    {
        SimpleMover shuriken;

        /*
        if (relPoint.x >= 0)
        {
            shuriken = Instantiate(shurikenPrefab, R_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Right;
        }
        else
        {
            shuriken = Instantiate(shurikenPrefab, L_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Left;
        }*/

        if (relPoint.x > 1)
        {
            shuriken = Instantiate(shurikenPrefab, R_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Right;
        }
        else if (relPoint.x < -1)
        {
            shuriken = Instantiate(shurikenPrefab, L_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Left;
        }
        else
        {
            shuriken = Instantiate(shurikenPrefab, R_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Middle;
            //shuriken.rotateDir = Vector3.zero;

            shuriken.transform.Rotate(0, 0, Random.Range(-180,180));
           // shuriken.rotateDir = Vector3.left * 20;
        }

        shuriken.SetTargetPosition(throwDir);
    }

    private void OnDestroy()
    {
        _inputService.OnColliderClick -= GetThrowDirection;
    }
}
