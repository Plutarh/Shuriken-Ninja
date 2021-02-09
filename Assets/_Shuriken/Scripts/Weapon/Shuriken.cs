using BzKovSoft.ObjectSlicerSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Shuriken : MonoBehaviour , IThrowable
{

    [SerializeField] BzKnife slicer;


    [Header("Movement")]
    public Vector3 moveDir;
    public float moveSpeed;

    public Vector3 startPos;
    public Vector3 endPos;

    public Vector3 startAnchor;
    public Vector3 endAnchor;

    [Header("Rotation")]
    public Vector3 mainRotateDir;
    public Vector3 secondaryRotateDir;

    public float mainRotateSpeed;
    public float secondRotateSpeed;

    public GameObject mainRotateObject;
    public GameObject secondaryRotateObject;

    [Header("Distance")]    
    public float vectorLength;

    float tParam;
    bool bezieMove;

    public EMoveType moveType;

    void Awake()
    {
        slicer.OnSliceBegin += BeginSlice;
    }
    
    void Start()
    {
        bezieMove = true;
        
    }

    
    void Update()
    {
        Movement();
        Rotation();
    }

    void BeginSlice()
    {

    }

    public void SetMoveType(EMoveType type)
    {
        moveType = type;
    }

    public void SetEndPosition(Vector3 _endPos)
    {
        endPos = _endPos;

        startPos = transform.position;
        vectorLength = (endPos - startPos).magnitude;

        startAnchor = startPos;
        endAnchor = endPos;
    }

    public void SetMoveDirection(Vector3 _dir)
    {
        moveDir = _dir;
    }

    public void SetStartPosition(Vector3 _startPos)
    {
       
    }

    void Movement()
    {
        switch (moveType)
        {
            case EMoveType.Free:
                MoveByBezieCurve();
                break;
            case EMoveType.Target:
                MoveToDirection();
                break;
        }
    }

    void MoveByBezieCurve()
    {
        tParam += Time.deltaTime * moveSpeed;
        if (tParam < 1)
        {
            transform.position = BezieCurve.GetPointOnBezierCurve
             (startPos
             , startAnchor
             , endAnchor
             , endPos
             , tParam);
        }
    }

    void MoveToDirection()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    void Rotation()
    {
        Quaternion yaw = Quaternion.Euler(mainRotateDir * Time.deltaTime * mainRotateSpeed);
        mainRotateObject.transform.rotation = yaw * mainRotateObject.transform.rotation; 

        Quaternion pitch = Quaternion.Euler(secondaryRotateDir * Time.deltaTime * secondRotateSpeed);
        secondaryRotateObject.transform.rotation = secondaryRotateObject.transform.rotation * pitch;
    }

    void OnDestroy()
    {
        slicer.OnSliceBegin -= BeginSlice;
    }
}
