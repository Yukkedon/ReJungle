using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ScoreTextAnim : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI textMeshPro;

    
    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        GameManager.Instance.isStart = false;
        Initialize();
        Play(2.5f);
    }

    /// <summary>
    /// フェードインアニメーションの開始処理
    /// </summary>
    private void Initialize()
    {
        textMeshPro.characterSpacing = -50;
    }

    private void Play(float duration)
    {
        // 文字を変える関数呼び出し
        ChangeText();
        
        // 文字間隔を開ける
        DOTween.To(() => textMeshPro.characterSpacing, value => textMeshPro.characterSpacing = value, 10, duration)
            .SetEase(Ease.OutQuart);

          

        TextAnim(textMeshPro);     
    }   
    

    /// <summary>
    /// テキストの表示を変える関数
    /// ToDo: メイン完了後変更
    /// </summary>
    private void ChangeText()
    {
        if(GameManager.Instance.combo==GameManager.Instance.MAX_COMBO)
        {
            textMeshPro.text=("FULL COMBO!");
        }
        else if (GameManager.Instance.point >= (GameManager.Instance.MAX_COMBO * 0.8) * GameManager.Instance.PERFECT_POINT)
        {
            textMeshPro.text = ("Great!");
        }
        else if (GameManager.Instance.point >= (GameManager.Instance.MAX_COMBO * 0.6) * GameManager.Instance.PERFECT_POINT)
        {
            textMeshPro.text = ("Bad!");
        }
        else if (GameManager.Instance.point >= (GameManager.Instance.MAX_COMBO * 0.4) * GameManager.Instance.PERFECT_POINT)
        {
            textMeshPro.text = ("Oh My God!");
        }

        else
        {
            textMeshPro.text=("Miss");
        }
    }

    /// <summary>
    /// テキストアニメーション処理
    /// </summary>
    /// <param name="text">アニメーションさせたいテキスト(TMPro)</param>
    private void TextAnim(TextMeshProUGUI text)
    {
        DOTweenTMPAnimator animator = new DOTweenTMPAnimator(text);
        //Sequenceで全文字のアニメーションをまとめる
        var sequence = DOTween.Sequence();
        sequence.SetLoops(-1);//無限ループ設定
    
        //一文字ずつにアニメーション設定
        var duration = 0.2f;//1回辺りのTween時間
        for (int i = 0; i < animator.textInfo.characterCount; ++i)
        {
            sequence.Join(DOTween.Sequence()
            //上に移動して戻る
            .Append(animator.DOOffsetChar(i, animator.GetCharOffset(i) + new Vector3(0, 30, 0), duration).SetEase(Ease.OutFlash, 2))
            //同時に1.2倍に拡大して戻る
            .Join(animator.DOScaleChar(i, 1.2f, duration).SetEase(Ease.OutFlash, 2))
            //同時に360度回転
            .Join(animator.DORotateChar(i, Vector3.forward * -360, duration, RotateMode.FastBeyond360).SetEase(Ease.OutFlash))
            //同時に色を黄色にして戻す
            .Join(animator.DOColorChar(i, Color.yellow, duration * 0.5f).SetLoops(2, LoopType.Yoyo))
            //アニメーション後、1秒のインターバル設定
            .AppendInterval(1f)
            //開始は0.15秒ずつずらす
            .SetDelay(0.15f * i));
        }
    }
}
