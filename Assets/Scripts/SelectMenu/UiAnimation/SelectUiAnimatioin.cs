using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SelectUiAnimatioin : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] TextMeshProUGUI gorillaDialogue = default;

    [SerializeField] GameObject backGround1;
    [SerializeField] GameObject backGround2;
    
    [SerializeField] GameObject textPanel;
    
    [SerializeField] GameObject[] miniChara;
    
    [SerializeField] GameObject SelectButtons;

    public bool isStartAnimEnd = false;
    
    void Awake()
    {
        fade.cutoutRange = 1f;
        
        backGround1.transform.localScale = new Vector3(2f, 2f, 2f);
        backGround2.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        SelectButtons.transform.localScale = Vector3.zero;
        textPanel.transform.localPosition = new Vector3(0f, 300f, 0f);
        for (int i = 0; i < miniChara.Length; i++)
        {
            miniChara[i].transform.localScale = Vector3.zero;
        }
        SelectButtons.SetActive(false);

    }

    void Start()
    {
        SelectAnimStart();
    }

    void SelectAnimStart()
    {
        StartCoroutine(AnimStart());
        TextDOTweenAnim();
    }

    private void TextDOTweenAnim()
    {
        DOTweenTMPAnimator animator = new DOTweenTMPAnimator(gorillaDialogue);

        var sequence = DOTween.Sequence();

        sequence.SetLoops(-1);

        var duration = 0.2f;
        for (int i = 0; i < animator.textInfo.characterCount; ++i)
        {
            sequence.Join(DOTween.Sequence()

              .Append(animator.DOOffsetChar(i, animator.GetCharOffset(i) + new Vector3(0, 10, 0), duration).SetEase(Ease.OutFlash, 2))

              .Join(animator.DOScaleChar(i, 1.2f, duration).SetEase(Ease.OutFlash, 2))

              .Join(animator.DOColorChar(i, Color.gray, duration * 0.5f).SetLoops(2, LoopType.Yoyo))

              .AppendInterval(2f)

              .SetDelay(0.3f * i)
            );
        }
    }

    IEnumerator AnimStart()
    {
        fade.FadeOut(1f);
        yield return new WaitForSeconds(1f);

        backGround1.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);
        backGround2.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);
        
        textPanel.transform.DOLocalMoveY(-151f, 2.5f).SetEase(Ease.OutBounce);
        
        for (int i = 0; i < miniChara.Length; i++)
        {
            miniChara[i].transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutBounce);
        }
        yield return new WaitForSeconds(1f);
        SelectButtons.SetActive(true);
        SelectButtons.transform.DOScale(new Vector3(1, 1, 1), 1f);
        isStartAnimEnd = true;


        yield break;

    }
}
