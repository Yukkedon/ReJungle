using System;
using UniRx;
using UnityEngine;

public interface IInputEventProvider
{
    IObservable<bool> OnKeyStateD { get; }
    IObservable<bool> OnKeyStateF { get; }
    IObservable<bool> OnKeyStateJ { get; }
    IObservable<bool> OnKeyStateK { get; }


}
