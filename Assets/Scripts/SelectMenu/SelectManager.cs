using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SelectManager : MonoBehaviour
{

    [SerializeField] GameObject buttonPanel;
    [SerializeField] Fade fade;

    [SerializeField] Button yesbutton;
    [SerializeField] Button nobutton;
    [SerializeField] Button closebutton;

    [SerializeField] GameObject backPanel;
    [SerializeField] SoundSelect soundSelect;
    
    
    [SerializeField] EventSystem eventSystem;

    [SerializeField] SelectUiAnimatioin suAnimation;

    [SerializeField] SelectViewer sViewer;

    void Awake()
    {
        buttonPanel.transform.localScale = Vector3.zero;

        nobutton.onClick.AddListener(OnClickCloseButton);
        closebutton.onClick.AddListener(OnClickCloseButton);
        yesbutton.onClick.AddListener(OnClickYesButton);

        backPanel.SetActive(false);
    }

    void Update()
    {
        ChangeIsTrigger();
    }

    void ChangeIsTrigger()
    {
        if (!this.transform.GetComponent<BoxCollider2D>().isTrigger)
        {
            if (suAnimation.isStartAnimEnd)
            {
                BoxCollider2D bc2d = this.transform.GetComponent<BoxCollider2D>();
                //bc2d.isTrigger = true;
            }
        }
    }

    void OnClickCloseButton()
    {
        soundSelect.PlaySE((int)SoundSelect.SE.Cansel);

        backPanel.SetActive(false);

        buttonPanel.transform.DOScale(Vector3.zero, 0.2f);

        if (buttonPanel.transform.localScale == Vector3.zero)
        {
            buttonPanel.gameObject.SetActive(false);
        }
    }

    public void OnClickSelectMusic()
    {
        GameObject button = eventSystem.currentSelectedGameObject;

        soundSelect.PlaySE((int)SoundSelect.SE.Select);

        backPanel.SetActive(true);

        buttonPanel.gameObject.SetActive(true);
        buttonPanel.transform.DOScale(new Vector3(1, 1, 1), 0.2f);
    }

    void OnClickYesButton()
    {
        soundSelect.PlaySE((int)SoundSelect.SE.Select);
        fade.FadeIn(1f, () => SceneManager.LoadScene("MainScene"));
    }

    int nowMusicId = -1;
    void OnTriggerStay2D(Collider2D collision)
    {
        if (suAnimation.isStartAnimEnd)
        {
            var id = collision.gameObject.GetComponent<Cell>().id;
            GameManager.Instance.songName = sViewer.musicName[id];

            if (soundSelect.IsCheckEndBGM() || id != nowMusicId)
            {
                nowMusicId = id;
                soundSelect.PlayBGM(collision.gameObject.GetComponent<Cell>().id);
            }
        }
    }

}