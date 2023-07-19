using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainManager : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] TextMeshProUGUI countText;

    [SerializeField] SoundMain soundMain;
    [SerializeField] GameObject comboText;
    public string songName;

    public int MAX_RAITO_POINT = 5;
    public int MAX_DIGIT_POINT = 1000000;
    public int PERFECT_POINT = 5;

    public float startTime = 0;   // スタートボタンを押すまでの秒数を保存
    public float playerScore = 0;
    public float maxScore = 0;
    public int point = 0;

    int combo = 0;

    int perfect = 0;
    int great = 0;
    int bad = 0;
    int miss = 0;


    public bool isAnimStart = false;

    public void SetStartTime(float startTime)
    {
        this.startTime = startTime;
    }

    public void Start()
    {
        fade.cutoutRange = 1f;
        StartCoroutine(StartFade());

        songName = GameManager.Instance.songName;
        GameManager.Instance.ResetState();
    }

    IEnumerator StartFade()
    {
        yield return new WaitForSeconds(1f);
        fade.FadeOut(1f);
    }

    bool isStartFade = false;
    public void Update()
    {

        if (GameManager.Instance.isEnd && !isStartFade)
        {
            isStartFade = true;
            StartCoroutine(FadeOutStart());
            return;
        }

        if (!isAnimStart && !GameManager.Instance.isStart && Input.GetKeyDown(KeyCode.Space))
        {
            isAnimStart = true;
        }

        if (isAnimStart)
        {
            StartCoroutine(PushSpaceAnim());
        }


    }

    public void SetGameManagerScore()
    {
        GameManager.Instance.point = point;
        GameManager.Instance.combo = combo;
        GameManager.Instance.perfect = perfect;
        GameManager.Instance.great = great;
        GameManager.Instance.bad = bad;
        GameManager.Instance.miss = miss;
    }

    public void ResetCombo()
    {
        combo = 0;
    }
    public void AddCombo()
    {
        ComboAnim();
        combo++;
    }
    public int GetCombo()
    {
        return combo;
    }

    public int GetPoint()
    {
        return point;
    }


    // 0:perfect 1:great 2:bad 3:miss
    public void AddJudgeCount(int num)
    {
        switch (num)
        {
            case 0:
                perfect++;
                point += 5;
                break;
            case 1:
                great++;
                point += 3;
                break;
            case 2:
                bad++;
                point += 1;
                break;
            case 3:
                miss++;
                break;
        }
    }

    private void ComboAnim()
    {
        comboText.transform.DOPunchScale(new Vector3(1.05f, 1.05f, 1.05f), 0.1f).OnComplete(() => BaseScale());
    }

    private void BaseScale()
    {
        comboText.transform.localScale = Vector3.one;
    }

    IEnumerator PushSpaceAnim()
    {
        countText.text = "START";
        isAnimStart = false;
        countText.transform.DOScale(new Vector3(10,10,10) ,1);
        countText.DOPause();
        countText.transform.DOLocalRotate(new Vector3(0, 0, 720f), 1f, RotateMode.FastBeyond360).WaitForCompletion();
        countText.DOFade(0.0f, 1.0f).Play();
        
        yield return new WaitForSeconds(1f);
        SetStartTime(Time.time);
        GameManager.Instance.isStart = true;

        yield break;

    }


    IEnumerator FadeOutStart()
    {
        yield return new WaitForSeconds(1f);
        SetGameManagerScore();
        fade.FadeIn(1f, () => SceneManager.LoadScene("ResultScene"));
        yield return new WaitForSeconds(1f);
    }



}
