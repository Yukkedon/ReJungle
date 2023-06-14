using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class IInputEventProviderInpl : MonoBehaviour
{
    public IObservable<bool> OnTouchKey => _touchKey;

    private readonly Subject<bool> _touchKey = new Subject<bool>();

    // Start is called before the first frame update
    void Start()
    {
        _touchKey.AddTo(this);

        this.UpdateAsObservable()
            .Select(_ => Input.GetKey(KeyCode.D))
            .DistinctUntilChanged()
            .Subscribe(_ =>
            {
                if (_)
                {

                }
            }).AddTo(this);
    }
}
