using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPointMarker : MonoBehaviour
{

    public Vector3 size;
    public Mesh charMesh;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawMesh(charMesh, transform.position, transform.rotation);
        Gizmos.color = Color.white;
    }
}
