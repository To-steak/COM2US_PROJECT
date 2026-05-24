using System;
using UnityEngine;

public interface IInputReader
{
    Vector3 MoveInput { get; }
    bool IsRunning { get; }
    Vector2 MousePosition { get; }
}