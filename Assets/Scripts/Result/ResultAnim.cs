using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ResultAnim : MonoBehaviour
{
    // スコアパネル
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject resultTextObject;
    // Start is called before the first frame update
    [SerializeField] private GameObject[] miniChara;
    // 背景オブジェクト
    [SerializeField] private GameObject backGround1;
    [SerializeField] private GameObject backGround2;
    // Prefect,good,badのスコア表示
    [SerializeField] private GameObject prefectText;
    // リザルトロゴ
    [SerializeField] private TextMeshProUGUI resultText;
    // ボタン
    [SerializeField] private GameObject buttons;

    [SerializeField] Fade fade;
    
    /// <summary>
    /// 開始処理（Startより前に実行）
    /// </summary>
    private void Awake() 
    {
        fade.cutoutRange = 1f;
        // 移動するゲームオブジェクトの初期値設定
        scorePanel.transform.localPosition = new Vector3(-260f,-900f,0f);
        resultTextObject.transform.localPosition = new Vector3(1118f,165f,0f);
        // 大きさを変更するオブジェクトの初期値設定
        backGround1.transform.localScale = new Vector3(3.5f,3.5f,3.5f);
        backGround2.transform.localScale = new Vector3(3f,3f,3f);
        buttons.transform.localScale = new Vector3(0f, 0f, 0f);
        for (int i = 0; i<miniChara.Length;i++)
        {
            miniChara[i].transform.localScale = Vector3.zero;
        }
        // スコアロゴを非表示
        prefectText.SetActive(false);
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        //コルーチン開始
        StartCoroutine(AnimStart());   
    }

    /// <summary>
    ///  アニメーション処理
    /// </summary>
    /// <returns></returns>
    IEnumerator AnimStart()
    {
        fade.FadeOut(1f);
        backGround1.transform.DOScale(new Vector3(1f,1f,1f),1f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);//1秒待つ
        backGround2.transform.DOScale(new Vector3(1f,1f,1f),1f).SetEase(Ease.OutBounce); 
        yield return new WaitForSeconds(1f);
        scorePanel.transform.DOLocalMoveY(60f,2f);
        yield return new WaitForSeconds(1f);
        resultTextObject.transform.DOLocalMoveX(10f,2f).SetEase(Ease.OutBounce);
        for(int i = 0; i<miniChara.Length;i++)
        {
            miniChara[i].transform.DOScale(new Vector3(1f,1f,1f),2f).SetEase(Ease.OutBounce);
        }   
        yield return new WaitForSeconds(0.5f);
        buttons.transform.DOScale(new Vector3(1f, 1f, 1f), 1f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.5f);
        // スコアロゴを表示
        prefectText.SetActive(true);

         yield return new WaitForSeconds(2f);
        TextAnim(resultText);
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
            //同時に色を黄色にして戻す
            .Join(animator.DOColorChar(i, Color.gray, duration * 0.5f).SetLoops(2, LoopType.Yoyo))
            //アニメーション後、1秒のインターバル設定
            .AppendInterval(1f)
            //開始は0.15秒ずつずらす
            .SetDelay(0.15f * i));
        }
    }
}
