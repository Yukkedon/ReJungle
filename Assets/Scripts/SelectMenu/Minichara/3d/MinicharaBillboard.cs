using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


// TODO : デバッグ時にQuadモデルではなくする修正
public class MinicharaBillboard : MonoBehaviour
{
    // ミニキャラのマテリアル取得用GameObject
    [SerializeField] private GameObject CharaMate = default;

    // 表示するテクスチャ格納用
    [Tooltip("表示するテクスチャ")]
    [SerializeField] private Texture[] spriteImage;

    // テクスチャ判定用
    private bool isFront = true;

    // 裏返すスピード
    [Tooltip("裏返すスピード")]
    [SerializeField] private float speed = 1.0f;

    // 開始時のスケール
    // 反転処理を開始するときの大きさ
    private Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);

    // 中間地点のスケール (x = 0)
    // xが0なので見えない（反転時の大きさ）
    private Vector3 endScale = new Vector3(0f, 1.0f, 1.0f);

    // Scale格納用の大きさ
    private Vector3 localScale = new Vector3();

    // 補完用変数(0f～1fの間)
    private float tick;

    // 子オブジェクトの回転
    Quaternion childRotation;

    // y軸アニメーションの到達地点
    [Tooltip("y軸アニメーションの到達地点")]
    [SerializeField] private float arrivalPositionY = -0.5f;

    // arrivalPositionYまでの移動時間
    [Tooltip("裏返すまでの時間")]
    [SerializeField] private float arrivalTime = 2f;



    /// <summary>
    /// Awake処理
    /// </summary>
    private void Awake()
    {
        // 回転の初期値設定
        StartYRotation();
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        // 最初に表示するテクスチャの設定
        CharaMate.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", spriteImage[0]);

        StartCoroutine(Turn());
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // ビルボード処理
        // メインカメラの位置の取得
        Vector3 p = Camera.main.transform.position;
        p.y = transform.position.y;
        transform.LookAt(p);
    }


    /// <summary>
    /// doTweenアニメーション(Y軸のみ)
    /// </summary>
    /// <param name="arrivalPositionY">到達位置</param>
    /// <param name="duration">到達するまでの時間</param>
    private void DoTweenAnimMiniChara(float arrivalPositionY, float duration)
    {
        // y軸のみのアニメーション
        transform.DOLocalMoveY(arrivalPositionY, duration).
            SetEase(Ease.OutBounce).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// Spriteを反転させる関数
    /// スケールのxを秒数で変えている
    /// </summary>
    /// <returns></returns>
    IEnumerator Turn()
    {
        // トゥーンアニメーション開始
        DoTweenAnimMiniChara(arrivalPositionY, arrivalTime);

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

                CharaMate.transform.localScale = localScale;

                yield return null;
            }

            // 裏表を変える
            isFront = !isFront;

            // spriteを変える
            CahngeTexture();

            // 補完値の初期化
            tick = 0f;

            // (1/speed)秒で中間から最後までひっくり返す
            while (tick < 1.0f)
            {
                tick += Time.deltaTime * speed;

                localScale = Vector3.Lerp(endScale, startScale, tick);

                CharaMate.transform.localScale = localScale;

                yield return null;
            }
        }
    }

    /// <summary>
    /// テクスチャを切り替える関数（マテリアルのテクスチャを変えているだけ）
    /// </summary>
    private void CahngeTexture()
    {
        if (isFront == true)
        {
            CharaMate.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", spriteImage[0]);
        }
        else
        {
            CharaMate.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", spriteImage[1]);
        }
    }

    /// <summary>
    /// 子オブジェクトRotationのy軸を-180にする
    /// TODO:可能であればQuadから変更しこの処理を消す
    /// </summary>
    private void StartYRotation()
    {
        // 子オブジェクトの回転を取得
        childRotation = CharaMate.transform.rotation;

        childRotation.y = -180f;

        // 実際の子オブジェクトの回転に反映
        CharaMate.transform.rotation = childRotation;
    }
}
