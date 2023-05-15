using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// �~�j�L�����̔��]�ƃA�j���[�V��������
/// TODO : Addressable Assets System�ɕύX����\��
/// </summary>
public class MiniCharaMove : MonoBehaviour
{
    // �\������X�v���C�g�i�[�p
    [SerializeField] private Sprite[] spriteImage;

    // Sprite����p
    private bool isFront = true;

    // ���Ԃ��X�s�[�h
    [Tooltip("���Ԃ��X�s�[�h")]
    [SerializeField] private float speed = 1.0f;

    // ����Image��RectTransform
    [SerializeField] RectTransform rectTransform;

    // ���̃I�u�W�F�N�g��Image
    [SerializeField] private Image miniChara;

    // �J�n���̃X�P�[��
    // ���]�������J�n����Ƃ��̑傫��
    private Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);

    // ���Ԓn�_�̃X�P�[�� (x = 0)
    // x��0�Ȃ̂Ō����Ȃ��i���]���̑傫���j
    private Vector3 endScale = new Vector3(0f, 1.0f, 1.0f);

    // RectTransform�i�[�p�̑傫��
    private Vector3 localScale = new Vector3();

    // �⊮�p�ϐ�(0f�`1f�̊�)
    private float tick;

    // �W�����v����
    [SerializeField] bool isJump ;
    
    //
    [Tooltip("�W�����v��")]
    [SerializeField] float jumpPower;

    [Tooltip("�W�����v������Ԋu")]
    [SerializeField] float duration;
    

    /// <summary>
    /// �J�n����
    /// </summary>
    private void Start()
    {
        miniChara.sprite = spriteImage[0];
        StartCoroutine(TurnAndJump());
    }

    /// <summary>
    /// Sprite�𔽓]������֐�
    /// �X�P�[����x��b���ŕς��Ă���
    /// </summary>
    /// <returns></returns>
    IEnumerator TurnAndJump()
    {
        if (isJump == true)
        {
            DoTweenAnimMiniChara(jumpPower, duration);
        }
        // �������[�v
        while (true)
        {
            yield return new WaitForSeconds(2f);
            tick = 0f;
            // (1/speed)�b�Œ��Ԓn�_�܂łЂ�����Ԃ�
            while (tick < 1.0f)
            {
                tick += Time.deltaTime * speed;
                // ���`���
                localScale = Vector3.Lerp(startScale, endScale, tick);

                rectTransform.localScale = localScale;

                yield return null;
            }

            // ���\��ς���
            isFront = !isFront;

            // sprite��ς���
            ChangeImage();

            // �⊮�l�̏�����
            tick = 0f;

            // (1/speed)�b�Œ��Ԃ���Ō�܂łЂ�����Ԃ�
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
    /// sprite��؂�ւ���֐�
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
    /// doTween�A�j���[�V����(Y���̂�)
    /// DOTO:������ύX����\������
    /// /// <param name="arrivalPositionY">���B�ʒu</param>
    /// <param name="duration">���B����܂ł̎���</param>
    /// </summary>
    private void DoTweenAnimMiniChara(float jopmPower, float duration)
    {
        // y���݂̂̃A�j���[�V����
        rectTransform.DOJump(new Vector3(
            transform.position.x, 
            transform.position.y,
            transform.position.z),
            jopmPower,1,duration).
            SetLoops(-1, LoopType.Yoyo);
    }
}

