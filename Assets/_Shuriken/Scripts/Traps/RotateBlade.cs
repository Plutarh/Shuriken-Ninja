using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBlade : Trap
{

    public GameObject rotateObject;
    public ETrapState trapState;
    public float rotateSpeed;
    public Vector3 stopOffset;

    public Transform target;
    public float angle;

    void Start()
    {
        Vector3 relative = transform.InverseTransformPoint(target.position);
        angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;


        //StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(2);
        ChangeState(ETrapState.Action);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine();
    }
    public void ActionState() => ChangeState(ETrapState.Action);
    public void ChangeState(ETrapState newState)
    {
        if (trapState == newState) return;
        trapState = newState;

        switch (newState)
        {
            case ETrapState.Wait:
                break;
            case ETrapState.Action:
                break;
            case ETrapState.Done:
                break;
        }
    }

    void StateMachine()
    {
        switch (trapState)
        {
            case ETrapState.Wait:
                break;
            case ETrapState.Action:

              
                Quaternion rotationTarget = Quaternion.LookRotation(target.localPosition - rotateObject.transform.localPosition);
                rotateObject.transform.localRotation = Quaternion.RotateTowards(rotateObject.transform.localRotation, rotationTarget, rotateSpeed * Time.deltaTime);
              
                if (rotateObject.transform.localRotation == rotationTarget)
                {
                    rotationTarget = Quaternion.LookRotation(target.localPosition - rotateObject.transform.localPosition) * Quaternion.Euler(stopOffset);
                    rotateObject.transform.localRotation = rotationTarget;
                    ChangeState(ETrapState.Done);
                }
              
                   
                break;
            case ETrapState.Done:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine
    }

}

public enum ETrapState
{
    Wait,
    Action,
    Done
}
