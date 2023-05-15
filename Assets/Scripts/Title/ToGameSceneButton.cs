using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ToGameSceneButton : MonoBehaviour
{   
    // 点滅頻度
    private float time;
    // 点滅スピード
    private float speed = 1.0f;
    // 点滅させたいUI格納
    [Tooltip("点滅させたいUI（ここにアタッチ）")]
    [SerializeField] private GameObject tapText;
    
    // タイトルロゴ
    [Tooltip("タイトルロゴ（ここにアタッチ）")]
    [SerializeField] private GameObject titleLogo;

    public AudioClip sound1;
    AudioSource audioSource;
    [SerializeField] Fade fade;
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        // タイトルロゴの初期の大きさ設定
        titleLogo.transform.localScale = Vector3.zero;
        // タップテキストを非表示に
        tapText.SetActive(false);
    }
    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        //Componentを取得
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(TitleLogoAnim());
    }
    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        // 点滅
        tapText.gameObject.GetComponent<Image>().color =
            GetAlphaColor(tapText.gameObject.GetComponent<Image>().color);
        
        if (Input.GetMouseButtonDown(0))
        {
            
            if (tapText.activeSelf)
            {
                audioSource.PlayOneShot(sound1);
                StartCoroutine(TapText());
            }
        }
    }
    /// <summary>
    /// Alpha値を更新してColorを返す点滅処理
    /// </summary>
    /// <param name="color"></param>
    Color GetAlphaColor(Color color)
    {
        time += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(time) * 0.5f + 0.5f;
        return color;
    }

    /// <summary>
    /// 「タップ」を押した時のコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator TapText()
    {
        // 点滅速度の上昇
        speed = 4f;
        // 1秒待つ
        yield return new WaitForSeconds(1f);
        // セレクトへ
        fade.FadeIn(1f, () => SceneManager.LoadScene("SelectScene"));
        
    }

    /// <summary>
    /// タイトルロゴの拡大処理
    /// </summary>
    /// <returns></returns>
    IEnumerator TitleLogoAnim()
    {
        yield return new WaitForSeconds(0.5f);
        titleLogo.transform.DOScale(new Vector3(1f, 1f, 1f), 1f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1.2f);
        tapText.SetActive(true);
    } 
}
