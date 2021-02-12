using System;
using UnityEngine;

public interface IInputService
{
    event Action<Vector3,Collider> OnColliderClick;
    event Action<Vector3> OnNonColliderClick;
}