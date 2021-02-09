using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowable 
{
    void SetMoveDirection(Vector3 dir);
    void SetStartPosition(Vector3 startPos);
    void SetEndPosition(Vector3 endPos);

    void SetMoveType(Shuriken.EMoveType mType);
}
