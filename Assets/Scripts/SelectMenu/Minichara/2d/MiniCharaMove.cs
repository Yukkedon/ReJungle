using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ミニキャラの反転とアニメーション処理
/// TODO : Addressable Assets Systemに変更する予定
/// </summary>
public class MiniCharaMove : MonoBehaviour
{
    // 表示するスプライト格納用
    [SerializeField] private Sprite[] spriteImage;

    // Sprite判定用
    private bool isFront = true;

    // 裏返すスピード
    [Tooltip("裏返すスピード")]
    [SerializeField] private float speed = 1.0f;

    // このImageのRectTransform
    [SerializeField] RectTransform rectTransform;

    // このオブジェクトのImage
    [SerializeField] private Image miniChara;

    // 開始時のスケール
    // 反転処理を開始するときの大きさ
    private Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);

    // 中間地点のスケール (x = 0)
    // xが0なので見えない（反転時の大きさ）
    private Vector3 endScale = new Vector3(0f, 1.0f, 1.0f);

    // RectTransform格納用の大きさ
    private Vector3 localScale = new Vector3();

    // 補完用変数(0f～1fの間)
    private float tick;

    // ジャンプ判定
    [SerializeField] bool isJump ;
    
    //
    [Tooltip("ジャンプ力")]
    [SerializeField] float jumpPower;

    [Tooltip("ジャンプをする間隔")]
    [SerializeField] float duration;
    

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        miniChara.sprite = spriteImage[0];
        StartCoroutine(TurnAndJump());
    }

    /// <summary>
    /// Spriteを反転させる関数
    /// スケールのxを秒数で変えている
    /// </summary>
    /// <returns></returns>
    IEnumerator TurnAndJump()
    {
        if (isJump == true)
        {
            DoTweenAnimMiniChara(jumpPower, duration);
        }
        // 無限ループ
        while (true)
        {
            yield return new WaitForSeconds(2f);
            tick = 0f;
            // (1/speed)秒で中間地点までひっくり返す
            while (tick < 1.0f)
            {
                tick += Time.deltaTime * speed;
                // 線形補間
                localScale = Vector3.Lerp(startScale, endScale, tick);

                rectTransform.localScale = localScale;

                yield return null;
            }

            // 裏表を変える
            isFront = !isFront;

            // spriteを変える
            ChangeImage();

            // 補完値の初期化
            tick = 0f;

            // (1/speed)秒で中間から最後までひっくり返す
            while (tick < 1.0f)
            {
                tick += Time.deltaTime * speed;

                localScale = Vector3.Lerp(endScale, startScale, tick);

                rectTransform.localScale = localScale;

                yield return null;
            }

        }
    }

    /// <summary>
    /// spriteを切り替える関数
    /// </summary>
    private void ChangeImage()
    {
        if (isFront == true)
        {
            miniChara.sprite = spriteImage[Random.Range(0,spriteImage.Length)];
        }
        else
        {
            miniChara.sprite = spriteImage[Random.Range(0, spriteImage.Length)];
        }
    }

    /// <summary>
    /// doTweenアニメーション(Y軸のみ)
    /// DOTO:動きを変更する可能性あり
    /// /// <param name="arrivalPositionY">到達位置</param>
    /// <param name="duration">到達するまでの時間</param>
    /// </summary>
    private void DoTweenAnimMiniChara(float jopmPower, float duration)
    {
        // y軸のみのアニメーション
        rectTransform.DOJump(new Vector3(
            transform.position.x, 
            transform.position.y,
            transform.position.z),
            jopmPower,1,duration).
            SetLoops(-1, LoopType.Yoyo);
    }
}

