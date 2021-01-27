using System;
using UnityEngine;

public interface IInputService
{
    event Action<Vector3> OnClick;
}