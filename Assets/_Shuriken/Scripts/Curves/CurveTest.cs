using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CurveTest : MonoBehaviour
{

    public Transform startPoint;
    public Transform endPoint;
    public Transform startPointAnchor;
    public Transform endPointAnchor;

    public GameObject shuriken;

    [Range(0,1)]
    public float tParam;
    public float speed;

    public bool coroutineAloud;

    public Vector3 shurPos;
    public Transform[] routes;



    private void OnEnable()
    {
        shuriken.transform.position = BezieCurve.GetPointOnBezierCurve(startPoint.position, startPointAnchor.position, endPointAnchor.position, endPoint.position, tParam);
    }

    void Start()
    {
       
     
    }

    void Update()
    {
        tParam += Time.deltaTime * speed;
        if(tParam < 1)
            shuriken.transform.position = BezieCurve.GetPointOnBezierCurve(startPoint.position, startPointAnchor.position, endPointAnchor.position, endPoint.position, tParam);

    }

    private void OnDrawGizmos()
    {
        Handles.DrawBezier(startPoint.position, endPoint.position, startPointAnchor.position, endPointAnchor.position, Color.red, null, 2);
    }
}

