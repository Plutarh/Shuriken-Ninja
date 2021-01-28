using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerController : Pawn
{
    public Vector3 shurikenOffset;
    public SimpleMover shurikenPrefab;

    public Transform shurikenSpawnPos;

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
    void GetThrowDirection(Vector3 direction)
    {
        throwDir = direction - (shurikenSpawnPos.position);
        animator.SetTrigger("Throw");
    }

    public Vector3 throwDir;

    public void ThrowShurikenByAnimator()
    {
        ThrowShuriken(throwDir.normalized);
    }

    void ThrowShuriken(Vector3 velocity)
    {
       

        var shuriken = Instantiate(shurikenPrefab, shurikenSpawnPos.position, Quaternion.identity);

        //shuriken.GetComponent<Rigidbody>().velocity = velocity;
        shuriken.moveDir = velocity;
    }

    private void OnDestroy()
    {
        _inputService.OnColliderClick -= GetThrowDirection;
    }
}
