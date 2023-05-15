using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ToGameSceneButton : MonoBehaviour
{   
    // �_�ŕp�x
    private float time;
    // �_�ŃX�s�[�h
    private float speed = 1.0f;
    // �_�ł�������UI�i�[
    [Tooltip("�_�ł�������UI�i�����ɃA�^�b�`�j")]
    [SerializeField] private GameObject tapText;
    
    // �^�C�g�����S
    [Tooltip("�^�C�g�����S�i�����ɃA�^�b�`�j")]
    [SerializeField] private GameObject titleLogo;

    public AudioClip sound1;
    AudioSource audioSource;
    [SerializeField] Fade fade;
    /// <summary>
    /// ����������
    /// </summary>
    private void Awake()
    {
        // �^�C�g�����S�̏����̑傫���ݒ�
        titleLogo.transform.localScale = Vector3.zero;
        // �^�b�v�e�L�X�g���\����
        tapText.SetActive(false);
    }
    /// <summary>
    /// �J�n����
    /// </summary>
    private void Start()
    {
        //Component���擾
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(TitleLogoAnim());
    }
    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        // �_��
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
    /// Alpha�l���X�V����Color��Ԃ��_�ŏ���
    /// </summary>
    /// <param name="color"></param>
    Color GetAlphaColor(Color color)
    {
        time += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(time) * 0.5f + 0.5f;
        return color;
    }

    /// <summary>
    /// �u�^�b�v�v�����������̃R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator TapText()
    {
        // �_�ő��x�̏㏸
        speed = 4f;
        // 1�b�҂�
        yield return new WaitForSeconds(1f);
        // �Z���N�g��
        fade.FadeIn(1f, () => SceneManager.LoadScene("SelectScene"));
        
    }

    /// <summary>
    /// �^�C�g�����S�̊g�又��
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
