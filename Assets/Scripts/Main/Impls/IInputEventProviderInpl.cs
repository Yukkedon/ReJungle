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

        // D�{�^������
         this.UpdateAsObservable()
            .Select(_ => Input.GetKey(KeyCode.D))
            .DistinctUntilChanged()
            .Skip(1)
            .Subscribe(_ =>
            {
                //�{�^���̏�Ԃ��ω������珈��
                if (_)
                {
                    _keyStateD.OnNext(true);
                }
                else
                {
                    _keyStateD.OnNext(false);
                }
            }).AddTo(this);
        
        // F�{�^������
         this.UpdateAsObservable()
            .Select(_ => Input.GetKey(KeyCode.F))
            .DistinctUntilChanged()
            .Skip(1)
            .Subscribe(_ =>
            {
                //�{�^���̏�Ԃ��ω������珈��
                if (_)
                {
                    _keyStateF.OnNext(true);
                }
                else
                {
                    _keyStateF.OnNext(false);
                }
            }).AddTo(this);
        
        // J�{�^������
         this.UpdateAsObservable()
            .Select(_ => Input.GetKey(KeyCode.J))
            .DistinctUntilChanged()
            .Skip(1)
            .Subscribe(_ =>
            {
                //�{�^���̏�Ԃ��ω������珈��
                if (_)
                {
                    _keyStateJ.OnNext(true);
                }
                else
                {
                    _keyStateJ.OnNext(false);
                }
            }).AddTo(this);
        
        // K�{�^������
         this.UpdateAsObservable()
            .Select(_ => Input.GetKey(KeyCode.K))
            .DistinctUntilChanged()
            .Skip(1)
            .Subscribe(_ =>
            {
                //�{�^���̏�Ԃ��ω������珈��
                if (_)
                {
                    _keyStateK.OnNext(true);
                }
                else
                {
                    _keyStateK.OnNext(false);
                }
            }).AddTo(this);



    }
}
