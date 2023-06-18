using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class IInputEventProviderInpl : MonoBehaviour
{
    public IObservable<bool> OnTouchKey => _keyState;

    private readonly Subject<bool> _keyState = new Subject<bool>();

    // Start is called before the first frame update
    void Start()
    {
        _keyState.AddTo(this);

        // Dボタン処理
        this.UpdateAsObservable()
            .Select(_ => Input.GetKey(KeyCode.D))
            .DistinctUntilChanged()
            .Skip(1)
            .Subscribe(_ =>
            {
                //ボタンの状態が変化したら処理

            }).AddTo(this);
    }
}
