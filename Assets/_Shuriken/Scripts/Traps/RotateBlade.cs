using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBlade : Trap
{

    public GameObject rotateObject;
    public ETrapState trapState;
    public float rotateSpeed;
    public Vector3 finalRotation;

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

                Quaternion rotationTarget = Quaternion.LookRotation(target.position - rotateObject.transform.position);
                //rotationTarget.y = 0;
                rotateObject.transform.rotation = Quaternion.RotateTowards(rotateObject.transform.rotation, rotationTarget, rotateSpeed * Time.deltaTime);
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
