using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMover : MonoBehaviour
{
    public Vector3 moveDir;
    public Vector3 mainRotateDir;
    public Vector3 secondaryRotateDir;

    public GameObject mainRotateObject;
    public GameObject secondaryRotateObject;

    public float moveSpeed;
    public float rotateSpeed;

    public Vector3 startPos;
    public Vector3 endPos;

    public Vector3 startAnchor;
    public Vector3 endAnchor;

    public float vectorLength;

    public EFlySide flySide = EFlySide.Right;
    public enum EFlySide
    {
        Right,
        Left,
        Middle
    }

    float tParam;
    bool bezieMove;

    void Start()
    {
        bezieMove = true;
        startPos = transform.position;
        vectorLength = (endPos - startPos).magnitude;

        if (vectorLength > 8)
        {
            moveSpeed = moveSpeed / 2;
        }
        if(vectorLength > 6)
        {
            switch (flySide)
            {
                case EFlySide.Right:
                    startAnchor = startPos + new Vector3(3, 0, vectorLength / 3);
                    break;
                case EFlySide.Left:
                    startAnchor = startPos + new Vector3(-3, 0, vectorLength / 3);
                    break;
                case EFlySide.Middle:
                    startAnchor = startPos + new Vector3(0, vectorLength / 8,0);
                    break;
            }

            endAnchor = endPos;
        }
        else
        {
            bezieMove = false;
            moveSpeed = 5;
            moveDir = (endPos - startPos).normalized;
        }
      

        //Destroy(gameObject,5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (bezieMove)
            MoveByBezieCurve();
        else
            SimpeMove();

        Rotation();
    }

    void MoveByBezieCurve()
    {
        tParam += Time.deltaTime * moveSpeed;
        if (tParam < 1)
            transform.position = BezieCurve.GetPointOnBezierCurve
                (startPos
                , startAnchor
                , endAnchor
                , endPos
                , tParam);
        else
        {
            if (moveDir == Vector3.zero) moveDir = endPos - startPos;
            transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        }


     
    }

    void SimpeMove()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    public float t1;
    public float t2;

    float rotates;
    void Rotation()
    {
        //mainRotateObject.transform.localEulerAngles = new Vector3(0,0,Mathf.PingPong(Time.time * t1, t2));
        secondaryRotateObject.transform.Rotate(secondaryRotateDir * rotateSpeed * Time.deltaTime);
       
       
    }

    public void SetTargetPosition(Vector3 pos)
    {
        endPos = pos;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (bezieMove)
        {
            UnityEditor.Handles.DrawBezier(startPos, endPos, startAnchor, endAnchor, Color.red, null, 2);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(startAnchor, 0.1f);
            Gizmos.DrawSphere(endAnchor, 0.1f);
        }
#endif
    }
}
