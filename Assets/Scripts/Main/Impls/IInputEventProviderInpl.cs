using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class IInputEventProviderInpl : MonoBehaviour,IInputEventProvider
{

    #region
    public IObservable<bool> OnKeyStateD => _keyStateD;
    public IObservable<bool> OnKeyStateF => _keyStateF;
    public IObservable<bool> OnKeyStateJ => _keyStateJ;
    public IObservable<bool> OnKeyStateK => _keyStateK;
    #endregion

    private readonly Subject<bool> _keyStateD = new Subject<bool>();
    private readonly Subject<bool> _keyStateF = new Subject<bool>();
    private readonly Subject<bool> _keyStateJ = new Subject<bool>();
    private readonly Subject<bool> _keyStateK = new Subject<bool>();
    
    // Start is called before the first frame update
    void Start()
    {
        _keyStateD.AddTo(this);
        _keyStateF.AddTo(this);
        _keyStateJ.AddTo(this);
        _keyStateK.AddTo(this);

        // Dボタン処理
         this.UpdateAsObservable()
            .Select(_ => Input.GetKey(KeyCode.D))
            .DistinctUntilChanged()
            .Skip(1)
            .Subscribe(_ =>
            {
                //ボタンの状態が変化したら処理
                if (_)
                {
                    _keyStateD.OnNext(true);
                    Debug.Log("おされた");
                }
                else
                {
                    _keyStateD.OnNext(false);
                    Debug.Log("はなれた");
                }
            }).AddTo(this);



    }
}
