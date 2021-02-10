using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowable 
{
    void SetMoveDirection(Vector3 dir);
    void SetStartPosition(Vector3 startPos);
    void SetTargetPosition(Vector3 tPos);

    void SetMoveType(Shuriken.EMoveType mType);
}
