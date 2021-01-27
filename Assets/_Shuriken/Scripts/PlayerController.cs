using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{

    public SimpleMover shurikenPrefab;

    IInputService _inputService;
    [Inject]
    void Construct(IInputService inputService)
    {
        _inputService = inputService;
        _inputService.OnClick += GetThrowDirection;
    }
    private void Awake()
    {
        
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
        Debug.Log($"{direction} dir");

        ThrowShuriken(direction.normalized);
    }

    void ThrowShuriken(Vector3 velocity)
    {
        var shuriken = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);

        //shuriken.GetComponent<Rigidbody>().velocity = velocity;
        shuriken.moveDir = velocity;
    }

    private void OnDestroy()
    {
        _inputService.OnClick -= GetThrowDirection;
    }
}
