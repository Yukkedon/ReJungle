using System;
using UniRx;
using UnityEngine;

public interface IInputEventProvider
{
    IObservable<bool> OnKeyState { get; }
}
