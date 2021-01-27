using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMover : MonoBehaviour
{
    public Vector3 moveDir;
    public Vector3 rotateDir;

    public GameObject rotateObject;

    public float moveSpeed;
    public float rotateSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        rotateObject.transform.Rotate(rotateDir * rotateSpeed * Time.deltaTime);
    }
}
